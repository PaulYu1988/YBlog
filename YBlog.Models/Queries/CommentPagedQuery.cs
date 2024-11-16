namespace YBlog.Models.Queries
{
    public class CommentPagedQuery : PagedQuery
    {
        public int? ArticleId { get; set; }
        public int? Status { get; set; }
        public int? UserId { get; set; }
    }
}
