using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        public string? Username { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        public string? Password { get; set; }
        public bool StayLogin { get; set; }
        [Required]
        public string? Captcha { get; set; }
    }
}
