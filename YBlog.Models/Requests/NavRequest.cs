using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class NavRequest
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        [Required]
        [StringLength(16, ErrorMessage = "文字最大长度16")]
        public string? Text { get; set; } = null!;
        [Required]
        public int? Target { get; set; }
        public string? Url { get; set; } = null!;
        public bool IsEnabled { get; set; }
        public int Sort { get; set; } = 50;
    }
}
