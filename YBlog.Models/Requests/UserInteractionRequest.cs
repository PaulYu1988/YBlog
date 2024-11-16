using System.ComponentModel.DataAnnotations;
using YBlog.Models.Enums;

namespace YBlog.Models.Requests
{
    public class UserInteractionRequest
    {
        [Required]
        public int? ArticleId { get; set; }
        public bool Checked { get; set; }
        [Required]
        public EnumUserInteractionTypes? Type { get; set; }
    }
}
