using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        public string? Username { get; set; }
        [StringLength(16, ErrorMessage = "最大长度16")]
        public string? Nickname { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        public string? Password { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        [Compare("Password", ErrorMessage = "两次密码不匹配")]
        public string? ConfirmPassword { get; set; }
        public bool StayLogin { get; set; }
        [Required]
        public string? Captcha { get; set; }
    }
}
