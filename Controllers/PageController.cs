
using Microsoft.AspNetCore.Mvc;
using ECampusDiscussion.EntityData;
using ECampusDiscussion.JwtAuth;
using ECampusDiscussion.Models;

namespace HRMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {

        private readonly MyDbContext dbContext;
        private readonly PermissionCheck permission;
        public PageController(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
            permission = new PermissionCheck(this.dbContext);
        }

        [HttpGet]
        public IActionResult Pages()
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();
            var pages = (from pm in dbContext.Page
                         join spm in dbContext.Page
                         on pm.ParentId equals spm.Id into tspm
                         from spm in tspm.DefaultIfEmpty()
                         select new RolePermission
                         {
                             Id = pm.Id,
                             Name = pm.Name,
                             Label = pm.Label,
                             Url = pm.Url,
                             Description = pm.Description,
                             IsFeature = pm.IsFeature,
                             ParentId = pm.ParentId,
                             Parent = spm == null ? pm.Name : spm.Name
                         })
                            .OrderBy(o => o.Parent)
                            .ThenBy(o => o.IsFeature)
                            .ThenBy(o => o.Name)
                            .ToList();
            return Ok(pages);
        }

        [HttpPost]
        public IActionResult CreatePage(Page newPage)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();
            if (newPage.IsFeature)
            {
                if (newPage.ParentId == null || newPage.ParentId == 0)
                {
                    return BadRequest("Select page for this feature");
                }
                else if (dbContext.Page.Any(f => f.ParentId == newPage.ParentId && f.Name == newPage.Name))
                {
                    return BadRequest("A feature with this name already exists in this page.");
                }
            }
            else
            {
                if (dbContext.Page.Any(f => f.IsFeature == false && f.Name == newPage.Name))
                {
                    return BadRequest("A page with this name already exists.");
                }
                if (string.IsNullOrWhiteSpace(newPage.Url) || string.IsNullOrWhiteSpace(newPage.Label))
                    return BadRequest("Url and Lable is required for page.");
                newPage.ParentId = null;
            }
            dbContext.Page.Add(newPage);
            dbContext.SaveChanges();
            permission.UpdatePage();
            return Ok(newPage);
        }

        [HttpPut]
        public IActionResult UpdatePage(RolePermission page)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();
            var db_page = dbContext.Page.Find(page.Id);
            if (db_page == null) return BadRequest();
            db_page.Label = page.Label;
            db_page.Url = page.Url;
            db_page.Description = page.Description;
            dbContext.Page.Update(db_page);
            dbContext.SaveChanges();
            return Ok(db_page);
        }

        [HttpDelete]
        public IActionResult DeletePage(int pageId)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();
            var page = dbContext.Page.Find(pageId);
            if (page == null) return BadRequest();

            var childPages = dbContext.Page.Where(p => p.ParentId == pageId).ToList();
            if (childPages.Count > 0)
            {
                dbContext.Page.RemoveRange(childPages);
                dbContext.SaveChanges();
            }
            dbContext.Page.Remove(page);
            dbContext.SaveChanges();
            permission.UpdatePage();
            return Ok(page);
        }

        [HttpGet]
        [Route("permission/{roleId}")]
        public IActionResult RolePermissions(int roleId)
        {
            //if (!permission.HasPermission(User, "Role")) return BadRequest("Permission not granted!");
            var rolePermissions = (from pm in dbContext.Page
                                   join spm in dbContext.Page
                                   on pm.ParentId equals spm.Id into tspm
                                   from spm in tspm.DefaultIfEmpty()
                                   join pp in dbContext.Permission.Where(pp => pp.RoleId == roleId)
                                   on pm.Id equals pp.PageId into tpp
                                   from pp in tpp.DefaultIfEmpty()
                                   select new RolePermission
                                   {
                                       Id = pm.Id,
                                       Name = pm.Name,
                                       Label = pm.Label,
                                       Url = pm.Url,
                                       Description = pm.Description,
                                       IsFeature = pm.IsFeature,
                                       ParentId = pm.ParentId,
                                       Parent = spm == null ? pm.Name : spm.Name,
                                       Permission = pp != null
                                   })
                                    .OrderBy(o => o.Parent)
                                    .ThenBy(o => o.IsFeature)
                                    .ThenBy(o => o.Name).ToList();
            return Ok(rolePermissions);
        }

        [HttpPost]
        [Route("permission/{roleId}")]
        public IActionResult UpdatePermissions(int roleId, List<PagePermission> pagePermissions)
        {
            try
            {
                //if (!permission.HasPermission(User, "Role")) return BadRequest();
                var existingPermissions = dbContext.Permission.Where(f => f.RoleId == roleId).ToList();

                var newPermissions = new List<Permission>();
                var permissionsToRemove = new List<Permission>();

                foreach (var pp in pagePermissions)
                {
                    var dbpp = existingPermissions.FirstOrDefault(f => f.PageId == pp.PageId);

                    if (dbpp == null && pp.Permission)
                    {
                        newPermissions.Add(new Permission
                        {
                            RoleId = roleId,
                            PageId = pp.PageId
                        });
                    }
                    else if (dbpp != null && !pp.Permission)
                    {
                        permissionsToRemove.Add(dbpp);
                    }
                }

                if (newPermissions.Any())
                {
                    dbContext.Permission.AddRange(newPermissions);
                }

                if (permissionsToRemove.Any())
                {
                    dbContext.Permission.RemoveRange(permissionsToRemove);
                }

                dbContext.SaveChanges();
                permission.UpdatePagePermission();
                return Ok("Permissions updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating permissions: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("parents")]

        public IActionResult ParentPages()
        {
            try
            {
                var parentPages = dbContext.Page.Where(f => f.IsFeature == false).Select(s => new { s.Id, s.Name }).ToArray();
                return Ok(parentPages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

    public class PagePermission
    {
        public int PageId { get; set; }
        public bool Permission { get; set; }
    }


    public class RolePermission
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Label { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public bool IsFeature { get; set; }
        public int? ParentId { get; set; }
        public string? Parent { get; set; }
        public bool Permission { get; set; }
        public List<RolePermission>? Features { get; set; }
    }
}
