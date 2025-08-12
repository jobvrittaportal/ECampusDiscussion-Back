using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECampusDiscussion.Models
{
    public class Permission_Table: Base
    {
        //[Key]
        //public int Permission_ID { get; set; }
        public required int Roles_ID { get; set; }
        [ForeignKey(nameof(Roles_ID))]
        public virtual Role_Mast? Role_Mast { get; set; }
        public required string Permissions { get; set; }

    }
}
