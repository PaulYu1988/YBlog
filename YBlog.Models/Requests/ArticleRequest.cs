using System.ComponentModel.DataAnnotations;
using YBlog.Models.Enums;

namespace YBlog.Models.Requests
{
    public class ArticleRequest
    {
        public int? Id { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? ArticleContent { get; set; }
        public string? Thumbnail { get; set; }
        public string? Tags { get; set; }
        [Required]
        public EnumArticleStatuses? Status { get; set; }
        public bool IsTop { get; set; }
    }
}
