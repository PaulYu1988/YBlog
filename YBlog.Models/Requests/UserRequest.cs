using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class UserRequest
    {
        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string? Nickname { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        [StringLength(12, MinimumLength = 2)]
        public string? Job { get; set; }
        [StringLength(300, MinimumLength = 8)]
        public string? Introduction { get; set; }
        public int? Id { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
    }
}
