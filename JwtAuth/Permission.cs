using ECampusDiscussion.EntityData;
using ECampusDiscussion.Models;
using System.Security.Claims;

namespace ECampusDiscussion.JwtAuth
{

    public class PermissionCheck
    {
        private readonly MyDbContext _db;
        private static List<Page>? Page;
        private static List<Permission>? PagePermission;
        private static List<User_Roles>? UserRole;

        public PermissionCheck(MyDbContext db)
        {
            _db = db;
            if (Page == null || PagePermission == null || UserRole == null)
            {
                UpdatePage();
                UpdatePagePermission();
                UpdateUserRole();
            }
        }

        public bool HasPermission(ClaimsPrincipal User, string pageName, string? featureName = null)
        {
            var login_id_claim = User.FindFirst("EmployeeID");
            if (login_id_claim == null || login_id_claim.Value == null) return false;

            if (Page != null && PagePermission != null && UserRole != null)
            {
                int loginId = int.Parse(login_id_claim.Value);
                int[] roles = UserRole.Where(f => f.Login_ID == loginId).Select(f => f.Role_ID).ToArray();

                int pageId = Page.Where(f => f.IsFeature == false && f.Name == pageName).Select(f => f.Id).SingleOrDefault();

                if (featureName != null)
                    pageId = Page.Where(f => f.ParentId == pageId && f.IsFeature == true && f.Name == featureName).Select(f => f.Id).SingleOrDefault();

                return PagePermission.Where(f => roles.Contains(f.RoleId) && f.PageId == pageId).Any();
            }
            else return false;
        }

        public void UpdatePagePermission()
        {
            PagePermission = _db.Permission.ToList();
        }
        public void UpdatePage()
        {
            Page = _db.Page.ToList();
        }
        public void UpdateUserRole()
        {
            UserRole = _db.User_Roles.ToList();
        }

    }
}
