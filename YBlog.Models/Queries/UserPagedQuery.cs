namespace YBlog.Models.Queries
{
    public class UserPagedQuery : PagedQuery
    {
        public string? Nickname { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
    }
}
