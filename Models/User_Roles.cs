using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECampusDiscussion.Models
{
    public class User_Roles: Base
    {
        public required int Login_ID { get; set; }
        [ForeignKey(nameof(Role))]
        public required int Role_ID { get; set; }

        public virtual Role_Mast Role { get; set; }
    }
}
