using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECampusDiscussion.Models
{
    public class Base
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public int? Created_By_Id { get; set; }
        public int? Updated_By_Id { get; set; }
        //[NotMapped]
        //public virtual Employee_Detail? Created_By { get; set; }
        //[NotMapped]
        //public virtual Employee_Detail? Updated_By { get; set; }
    }
}
