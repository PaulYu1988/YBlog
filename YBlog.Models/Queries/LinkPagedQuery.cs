namespace YBlog.Models.Queries
{
    public class LinkPagedQuery : PagedQuery
    {
        public string? Text { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
