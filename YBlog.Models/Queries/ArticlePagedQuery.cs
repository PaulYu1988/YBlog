namespace YBlog.Models.Queries
{
    public class ArticlePagedQuery : PagedQuery
    {
        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
    }
}
