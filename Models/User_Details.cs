using ECampusDiscussion.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECampusDiscussion.Models
{
    public class User_Details : Base
    {
        public required int Login_ID { get; set; }
        public required string Login_Name { get; set; }
        public required string Login_Password { get; set; }
        public string? User_Name { get; set; }
        public required string User_EmailID { get; set; }
        public string? User_PhoneNo { get; set; }
        public required bool User_Status { get; set; } = true;
    }
}
