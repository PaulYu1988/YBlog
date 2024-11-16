using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class CommentRequest
    {
        [Required]
        public int? ArticleId { get; set; }
        [Required]
        public string? CommentContent { get; set; } = null!;
        [Required]
        public string? Captcha { get; set; }
    }
}
