using ECampusDiscussion.EntityData;
using ECampusDiscussion.JwtAuth;
using ECampusDiscussion.Models;
using ECampusDiscussion.Utility_Function;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ECampusDiscussion.Controllers;
using ECampusDiscussion.EntityData;
using ECampusDiscussion.JwtAuth;
using ECampusDiscussion.Utility_Function;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ECampusDiscussionHRMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Employee_DetailController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly ITokenManager tokenManager;
        private readonly string connString = Common_Func.ConnectionString();
        private readonly PermissionCheck _permissionCheck;
        private readonly IWebHostEnvironment webHostEnvironment;

        public Employee_DetailController(MyDbContext db, ITokenManager tokenManager, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.tokenManager = tokenManager;
            this.webHostEnvironment = webHostEnvironment;
            _permissionCheck = new PermissionCheck(db);

        }



        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginDetails user)
        {
            try
            {
                
                var User = db.User_Details.SingleOrDefault(u => u.User_EmailID.ToLower().Trim() == user.User_EmailID.ToLower().Trim() && user.Login_Password == u.Login_Password);
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

                db.SaveChanges();


                return Ok(new
                {
                    isAdmin = user.User_EmailID == "admin" ? true : false,
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




        public class Hierarchy
        {
            public int Employee_Id { get; set; }
            public int Manager { get; set; }
            public int Team_Lead { get; set; }
        }





        public class ExportRequest
        {
            public bool? Employee_Details { get; set; }
            public bool? Family_Details { get; set; }
            public bool? Educational_Details { get; set; }
            public bool? Employer_Details { get; set; }
            public bool? Bank_Account_Details { get; set; }
            public string Employee_Id { get; set; }
        }

        public class EmployeeStatusUpdate
        {
            public int EmployeeId { get; set; }
            public int StatusId { get; set; }
        }

        public class LazyParams
        {
            public int first { get; set; }
            public int rows { get; set; }
            public int page { get; set; }
            public string? sortField { get; set; }
            public int sortOrder { get; set; }
        }
        public class EmployeeFilter
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public int? Designation { get; set; }
            public int? Department_ID { get; set; }
            public int? Company_ID { get; set; }
            public int? Branch_ID { get; set; }
            public int? Status { get; set; }
            public int? Shift { get; set; }
            public int? Payroll_Company_Id { get; set; }
            public bool? esic_Member { get; set; }

            public int? Business_Process_Id { get; set; }

        }
        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }
        public class ResetPasswordRequest
        {
            public string NewPassword { get; set; }
            public string Email { get; set; }
            //public string OTP { get; set; }
        }
        public class VerifyOTPRequest
        {
            public string Email { get; set; }
            public string OTP { get; set; }
        }

        public class LoginDetails
        {
            public string Login_Password { get; set; }

            public string User_EmailID { get; set; }
        }

        public class ChangePasswordRequest
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }


    }
    }
