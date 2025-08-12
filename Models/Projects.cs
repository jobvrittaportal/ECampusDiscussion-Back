using System.ComponentModel.DataAnnotations;

namespace ECampusDiscussion.Models
{
    public class Projects : Base
    {
        public required string Project_Name { get; set; }
    }
}
