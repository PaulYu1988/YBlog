using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        public string? Password { get; set; }
        [Required]

        [StringLength(16, MinimumLength = 8, ErrorMessage = "最小长度8，最大长度16")]
        public string? NewPassword { get; set; }
    }
}
