using System.ComponentModel.DataAnnotations.Schema;

namespace ECampusDiscussion.Models
{
  public class Discussion_Mast: Base
  {
    [Column(TypeName = "varchar(MAX)")]
    public required string Discussion { get; set; }
  }
}
