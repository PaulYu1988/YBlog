namespace YBlog.Models.Queries
{
    public class UserInteractionPagedQuery : PagedQuery
    {
        public int? UserId { get; set; }
        public int? Type { get; set; }
        public int? ArticleId { get; set; }
    }
}
