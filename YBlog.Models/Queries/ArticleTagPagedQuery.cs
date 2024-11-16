namespace YBlog.Models.Queries
{
    public class ArticleTagPagedQuery : PagedQuery
    {
        public int? TagId { get; set; }
    }
}
