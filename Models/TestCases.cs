
namespace ECampusDiscussion.Models
{
    public class TestCases : Base
    {
        public required int ProjectID { get; set; }
        public required string Module { get; set; }
        public required string SubModule { get; set; }
        public required string Scenario { get; set; }
        public required string Description { get; set; }
        public required bool Test_Type { get; set; }
        public required string Steps_To_Execute { get; set; }
        public required string Preconditions { get; set; }
        public required string Expected_Result { get; set; }
        public required string Actual_Result { get; set; }
        public required bool Status { get; set; }
        public string? Comment { get; set; }


    }
}
