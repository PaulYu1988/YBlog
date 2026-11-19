namespace YBlog.Models.Queries
{
    public class BotPagedQuery : PagedQuery
    {
        public string? Name { get; set; }
        public string? IdentificationCode { get; set; }
        public int? Type { get; set; }
    }
}
