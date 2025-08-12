
using ECampusDiscussion.EntityData;
using ECampusDiscussion.JwtAuth;
using ECampusDiscussion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ECampusDiscussion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_MastController : ControllerBase
    {
        readonly private MyDbContext db;
        private readonly ITokenManager tokenManager;
        private readonly PermissionCheck _permissionCheck;
        public User_MastController(MyDbContext db, ITokenManager tokenManager)
        {
            this.db = db;
            this.tokenManager = tokenManager;
            _permissionCheck = new PermissionCheck(db);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUser([FromQuery] string? lazyParams, [FromQuery] string? filter, [FromQuery] string? filters)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]);
                int employeeId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "EmployeeID").Value);

                List<int?> branch_ids = new List<int?>();

                bool forBranchRecord = _permissionCheck.HasPermission(User, "User", "View_Own_Branch");


                LazyParams lazyparams = JsonConvert.DeserializeObject<LazyParams>(lazyParams);
                UserMastFilter Filters = JsonConvert.DeserializeObject<UserMastFilter>(filters);
                var query = (from u in db.User_Details
                             select new
                             {
                                 Login_ID = u.Login_ID,
                                 //Id = u.Id,
                                 Login_Name = u.Login_Name,
                                 Created_On = u.Created_At,
                                 // Added_By = u.Added_By,
                                 User_EmailID = u.User_EmailID,
                                 Login_Password = u.Login_Password,
                                 User_Status = u.User_Status,
                             });



                if (filter != "null" && filter != null)
                {
                    query = query.Where(f => f.Login_Name.Contains(filter) || f.User_EmailID.Contains(filter) || f.Login_Name.Contains(filter));
                }


                switch (lazyparams.sortField)
                {
                    case "login_Name":
                        if (lazyparams.sortOrder == 1) query = query.OrderBy(sf => sf.Login_Name);
                        else query = query.OrderByDescending(sf => sf.Login_Name);
                        break;
                    case "email":
                        if (lazyparams.sortOrder == 1) query = query.OrderBy(sf => sf.User_EmailID);
                        else query = query.OrderByDescending(sf => sf.User_EmailID);
                        break;

                }
                if (filters != "" && Filters != null)
                {


                    if (Filters.Email != null && Filters.Email != "")
                    {
                        query = query.Where(item => item.User_EmailID.Contains(Filters.Email));
                    }
                    if (Filters.User_Name != null && Filters.User_Name != "")
                    {
                        query = query.Where(item => item.Login_Name.Contains(Filters.User_Name));
                    }

                    if (Filters.status != null)
                    {
                        query = query.Where(f => f.User_Status == Filters.status);
                    }



                }

                var users = query.Skip(lazyparams.first).Take(lazyparams.rows).ToArray();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Authorize]
        [Route("count")]
        public IActionResult GetUserCount([FromQuery(Name = "filter")] string? filter, [FromQuery] string? filters)
        {
            try
            {

                var token = Request.Headers["Authorization"].ToString();
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]);
                int employeeId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "EmployeeID").Value);


                List<int?> branch_ids = new List<int?>();
                /* for admin*/
                bool showAllRecords = _permissionCheck.HasPermission(User, "User", "View_All_Record");

                bool forBranchRecord = _permissionCheck.HasPermission(User, "User", "View_Own_Branch");

                UserMastFilter Filters = JsonConvert.DeserializeObject<UserMastFilter>(filters);
                var query = (from u in db.User_Details
                             select new
                             {
                                 Login_ID = u.Login_ID,
                                 //Id = u.Id,
                                 Login_Name = u.Login_Name,
                                 Created_On = u.Created_At,
                                 // Added_By = u.Added_By,
                                 User_EmailID = u.User_EmailID,
                                 Login_Password = u.Login_Password,
                                 User_Status = u.User_Status,
                             });


                if (filter != "null" && filter != null)
                {
                    query = query.Where(f => f.Login_Name.Contains(filter) || f.User_EmailID.Contains(filter) || f.Login_Name.Contains(filter));
                }

                if (filters != "" && Filters != null)
                {


                    if (Filters.Email != null && Filters.Email != "")
                    {
                        query = query.Where(item => item.User_EmailID.Contains(Filters.Email));
                    }
                    if (Filters.User_Name != null && Filters.User_Name != "")
                    {
                        query = query.Where(item => item.Login_Name.Contains(Filters.User_Name));
                    }
                }

                int count = query.Count();
                return Ok(count);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Authorize]
        public IActionResult AddUser(User_Details user, [FromQuery] string? roles, [FromQuery] string? branches, [FromQuery] string? companies)
        {
            try
            {

                JavaScriptSerializer js = new JavaScriptSerializer();
                Roles[] t_roles = js.Deserialize<Roles[]>(roles);
                Branches[] t_branches = js.Deserialize<Branches[]>(branches);  //RecruiterBranches
                Companies[] t_Assign_Companies = js.Deserialize<Companies[]>(companies);
                User_Details alreadyExist = db.User_Details.FirstOrDefault(f => f.User_EmailID == user.User_EmailID);
                //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
                if (alreadyExist != null)
                {
                    return BadRequest("Already Exist");
                }
                else
                {
                    var token = Request.Headers["Authorization"].ToString();
                    var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]);
                    int employeeId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "EmployeeID").Value);

                    user.Created_At = DateTime.Now;
                    user.Login_ID = alreadyExist.Id;

                    //  user.Added_By = employeeId;
                    db.User_Details.Add(user);
                    db.SaveChanges();

                    foreach (var role in t_roles)
                    {
                        db.User_Roles.Add(new User_Roles
                        {
                            Role_ID = role.Role_ID,
                            Login_ID = employeeId,
                            Created_By_Id = employeeId,
                            Created_At = DateTime.Now
                        });
                    }


                    db.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Authorize]
        public IActionResult UpdateUsers(INewUser user, [FromQuery] string roles, [FromQuery] string? branches, [FromQuery] string? companies)
        {
            try
            {
                User_Details alreadyExist = db.User_Details.FirstOrDefault(f => f.Login_ID == user.Login_ID);

                JavaScriptSerializer js = new JavaScriptSerializer();
                Roles[] t_roles = js.Deserialize<Roles[]>(roles);
                Branches[] t_branches = js.Deserialize<Branches[]>(branches);
                Companies[] t_Assign_Companies = js.Deserialize<Companies[]>(companies);



                if (alreadyExist == null)
                {
                    return BadRequest("User not Found");
                }
                alreadyExist.User_EmailID = user.User_EmailID;
                alreadyExist.Login_Name = user.Login_Name;
                alreadyExist.Login_Password = user.Login_Password;


                db.User_Details.Update(alreadyExist);
                db.SaveChanges(); 
                db.SaveChanges();

                db.User_Roles.RemoveRange(db.User_Roles.Where(f => f.Login_ID == user.Login_ID));

                db.SaveChanges();

                var token = Request.Headers["Authorization"].ToString();
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]);
                int employeeId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "EmployeeID").Value);
                foreach (var role in t_roles)
                {

                    db.User_Roles.Add(new User_Roles
                    {

                        Role_ID = role.Role_ID,
                        Login_ID = user.Login_ID,
                    });
                }

                db.SaveChanges();
                _permissionCheck.UpdateUserRole();
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("togglestatus")]
        public IActionResult ToggleStatus([FromQuery] int? employeeID)
        {
            try
            {
                int login_id = Int32.Parse(User.FindFirst("EmployeeID")?.Value);
                // if (!checkPermission.Role_Granted(login_id, "Employee", "Active_Inactive_Status")) return BadRequest("Permission is Not Granted");
                User_Details user_Details = db.User_Details.FirstOrDefault(f => f.Login_ID == employeeID);

                if (user_Details.User_Status == false || user_Details.User_Status == null)
                {
                    user_Details.User_Status = true;
                    //user_Details.Is_To_Delete = false;
                }
                else
                {
                    user_Details.User_Status = false;
                }
                db.User_Details.Update(user_Details);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("testing")]
        public IActionResult Testing()
        {
            try
            {
                return Ok("Tested Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GeneratePassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        [HttpPost]
        [Route("Login")]
        [Authorize]
        public IActionResult Login(LoginPayload user)
        {
            try
            {
                var User = db.User_Details.SingleOrDefault(u => u.User_EmailID.ToLower().Trim() == user.Email.ToLower().Trim() && user.Password == u.Login_Password);
                if (User == null)
                {
                    return BadRequest("Email or password is incorrect");
                }

                if (User.User_Status == false)
                {
                    return BadRequest(error: "Your Account is Inactive, Please Contact to Admin.");
                }

                var token = tokenManager.NewToken(User.Login_ID.ToString());

                int[] roleIds = db.User_Roles.Where(f => f.Login_ID == User.Login_ID).OrderBy(o => o.Role_ID).Select(c => c.Role_ID).ToArray();

                var rolePermissions = (from pm in db.Page
                                       join spm in db.Page
                                       on pm.ParentId equals spm.Id into tspm
                                       from spm in tspm.DefaultIfEmpty()
                                       join pp in db.Permission
                                       .Where(pp => roleIds.Contains(pp.RoleId))
                                       on pm.Id equals pp.PageId into tpp
                                       from pp in tpp.DefaultIfEmpty()
                                       select new
                                       {
                                           pm.Id,
                                           pm.Name,
                                           pm.Label,
                                           pm.Url,
                                           pm.Description,
                                           pm.IsFeature,
                                           pm.ParentId,
                                           Parent = spm == null ? pm.Name : spm.Name,
                                           Permission = pp != null,
                                           RoleId = pp != null ? pp.RoleId : (int?)null
                                       })
                                       .GroupBy(rp => rp.Id)
                                      .Select(g => g.First())
                                      .ToList()
                                      .Select(rp => new RolePermission
                                      {
                                          Id = rp.Id,
                                          Name = rp.Name,
                                          Label = rp.Label,
                                          Url = rp.Url,
                                          Description = rp.Description,
                                          IsFeature = rp.IsFeature,
                                          ParentId = rp.ParentId,
                                          Parent = rp.Parent,
                                          Permission = rp.Permission
                                      }).ToList();


                return Ok(new
                {
                    isAdmin = user.Email == "admin" ? true : false,
                    message = "Login successful",
                    token = token,
                    employeeId = User.Login_ID,
                    employeeName = User.Login_Name,
                    roleId = roleIds,
                    permissions = rolePermissions,
                    isFirstLogin = false,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

    public class LoginPayload
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }



    public class LoginObj
    {
        public int? Id { get; set; }
        public string? Login_Name { get; set; }
        public string? Token { get; set; }
        public List<Roles>? Roles { get; set; }
        public string? Permissions { get; set; }

    }
    public class Roles
    {
        public int Role_ID { get; set; }
    }
    public class Branches
    {
        public int Branch_ID { get; set; }
    }
    public class Companies
    {
        public int Company_Id { get; set; }
    }

    public class UserMastFilter
    {
        public int? company { get; set; }
        public int? branch { get; set; }
        public string? Email { get; set; }
        public string? User_Name { get; set; }
        public bool? status { get; set; }
        public bool? isToDelete { get; set; }

    }

    public class INewUser
    {
        public int ID { get; set; }
        public int Login_ID { get; set; }
        public string? Login_Name { get; set; }
        public string? Login_Password { get; set; }
        public string? User_EmailID { get; set; }
        public int[]? Roles { get; set; }
        public int Company { get; set; }
        public int Branch { get; set; }
        public int[]? Assign_Branches { get; set; }

    }

    public class INewUser1
    {
        public string Login_Name { get; set; }
        public string User_EmailID { get; set; }
        public int Company { get; set; }
        public int Branch { get; set; }
    }

    public class LazyParams
    {
        public int first { get; set; }
        public int rows { get; set; }
        public int page { get; set; }
        public string? sortField { get; set; }
        public int sortOrder { get; set; }
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
