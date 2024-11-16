namespace YBlog.Models.Queries
{
    public class UserInteractionViewPagedQuery : PagedQuery
    {
        public string? Title { get; set; }
        public int? UserId { get; set; }
        public int? Type { get; set; }
    }
}
