using System.ComponentModel.DataAnnotations;
using System.Security;

namespace ECampusDiscussion.Models
{
    public class Role_Mast : Base
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        public string? Desc { get; set; }
        public virtual List<User_Roles>? RoleUsers { get; set; }
        public virtual List<Permission>? RolePermissions { get; set; }
    }
}
