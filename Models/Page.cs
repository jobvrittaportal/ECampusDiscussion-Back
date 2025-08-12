using System.ComponentModel.DataAnnotations;

namespace ECampusDiscussion.Models
{
    public class Page : Base
    {
        [MaxLength(255)]
        public required string Name { get; set; }
        [MaxLength(255)]
        public string? Label { get; set; }
        [MaxLength(255)]
        public string? Url { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        public bool IsFeature { get; set; }
        public int? ParentId { get; set; }
        public virtual List<Permission>? Permission { get; set; }
    }
}
