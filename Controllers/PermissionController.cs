
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECampusDiscussion.EntityData;
using ECampusDiscussion.Models;

namespace Job_Vritta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly MyDbContext db;
        public PermissionsController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        [Authorize]
        [Route("addpermission")]
        public IActionResult CreateRolePermission([FromBody] Permission_Table? user_permission, [FromQuery] int roles_ID)
        {
            if (user_permission == null)
            {
                return BadRequest("RoleDto is required.");
            }
            try
            {
                var permission = db.Permission_Table.FirstOrDefault(p => p.Roles_ID == roles_ID);

                if (permission == null)
                {

                    var permissions = new Permission_Table
                    {
                        Permissions = user_permission.Permissions,
                        Roles_ID = roles_ID,

                    };

                    db.Permission_Table.Add(permissions);
                    db.SaveChanges();
                }
                else
                {
                    permission.Permissions = user_permission.Permissions;
                    db.Permission_Table.Update(permission);
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getRoles")]
        public IActionResult GetAllRolePermissions([FromQuery(Name = "role_id")] int? role_id)
        {

            try
            {
                var rolesWithPermissions = (from role in db.Role_Mast
                                            join permission in db.Permission_Table on role.Id equals permission.Roles_ID
                                            select new
                                            {
                                                Permission_ID = permission.Id,
                                                Role_ID = permission.Roles_ID,
                                                Role_Name = role.Name,
                                                permissions = permission.Permissions
                                            }).Where(f => f.Role_ID == role_id).SingleOrDefault();


                return Ok(rolesWithPermissions);

                // return Ok(new { name = rolesWithPermissions });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
