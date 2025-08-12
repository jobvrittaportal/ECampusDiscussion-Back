using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using System.Text.Json;
using ECampusDiscussion.EntityData;
using ECampusDiscussion.Models;

namespace Job_Vritta.Controllers
{
    [Route("api/[controller]")]

    [ApiController]

    public class RoleController(MyDbContext db) : ControllerBase

    {

        readonly private MyDbContext db = db;

        [HttpGet]

        [Authorize]

        public IActionResult GetRoles([FromQuery] string? text, [FromQuery] string? lazyParams)

        {

            try

            {

                int first = 0, rows = 10;

                string sortField = "Id";

                int sortOrder = -1;

                if (!string.IsNullOrEmpty(lazyParams))

                {

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var parsed = System.Text.Json.JsonSerializer.Deserialize<LazyParams>(lazyParams, options);

                    if (parsed != null)

                    {

                        first = parsed.First;

                        rows = parsed.Rows;

                        sortField = parsed.SortField ?? "Id";

                        sortOrder = parsed.SortOrder ?? -1;

                    }

                }

                var query = db.Role_Mast.AsQueryable();

                if (!string.IsNullOrEmpty(text))

                {

                    query = query.Where(r => r.Name.Contains(text));

                }

                var totalRecords = query.Count();

                query = query.OrderByDynamic(sortField, sortOrder == 1);

                var result = query.Skip(first).Take(rows).ToList();

                return Ok(new { roles = result, count = totalRecords });

            }

            catch (Exception ex)

            {

                return StatusCode(500, new { error = ex.Message });

            }

        }


        [HttpPost]

        [Authorize]

        public IActionResult AddRole(Role_Mast role)

        {

            try

            {

                Role_Mast? alreadyExist = db.Role_Mast.SingleOrDefault(f => f.Name == role.Name);

                if (alreadyExist != null)

                {

                    return BadRequest("Already Exist");

                }

                else

                {

                    db.Role_Mast.Add(role);

                    db.SaveChanges();

                    return Ok(new { role });

                }

            }

            catch (Exception ex)

            {

                return BadRequest(ex.Message);

            }

        }

        [HttpPut("{id}")]

        public IActionResult EditRole(int id, [FromBody] Role_Mast updatedData)

        {

            try

            {

                if (id <= 0)

                {

                    return BadRequest(new { error = "Role_Mast ID is required" });

                }

                if (string.IsNullOrWhiteSpace(updatedData.Name))

                {

                    return BadRequest(new { error = "RoleName is required" });

                }

                var role = db.Role_Mast.SingleOrDefault(r => r.Id == id);

                if (role == null)

                {

                    return NotFound(new { error = "Role_Mast not found" });

                }

                role.Name = updatedData.Name;

                role.Desc = updatedData.Desc;

                db.SaveChanges();

                return Ok(new { role });

            }

            catch (Exception ex)

            {

                return StatusCode(500, new { error = ex.Message });

            }

        }

        [HttpDelete("{id}")]

        [Authorize]

        public IActionResult DeleteRole(int id)

        {

            try

            {

                var role = db.Role_Mast.SingleOrDefault(r => r.Id == id);

                if (role == null)

                {

                    return NotFound(new { error = "Role_Mast not found" });

                }

                db.Role_Mast.Remove(role);

                db.SaveChanges();

                return Ok(new { message = "Role_Mast deleted successfully" });

            }

            catch (Exception ex)

            {

                return StatusCode(500, new { error = ex.Message });

            }

        }

        [HttpGet("dropdown")]

        [Authorize]

        public IActionResult GetRoleDropdown()

        {

            try

            {

                var roles = db.Role_Mast.Select(r => new { r.Id, r.Name }).ToList();

                return Ok(roles);

            }

            catch (Exception ex)

            {

                return StatusCode(500, new

                {

                    success = false,

                    error = ex.Message

                });

            }

        }

    }

    public class LazyParams

    {

        public int First { get; set; }

        public int Rows { get; set; }

        public int Page { get; set; }

        public string? SortField { get; set; }

        public int? SortOrder { get; set; } // 1 = ASC, -1 = DESC

    }

    public static class IQueryableExtensions

    {

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortField, bool ascending)

        {

            var param = Expression.Parameter(typeof(T), "x");

            var property = Expression.PropertyOrField(param, sortField);

            var lambda = Expression.Lambda(property, param);

            string methodName = ascending ? "OrderBy" : "OrderByDescending";

            var method = typeof(Queryable).GetMethods()

                .First(m => m.Name == methodName && m.GetParameters().Length == 2)

                .MakeGenericMethod(typeof(T), property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;

        }

    }


}


