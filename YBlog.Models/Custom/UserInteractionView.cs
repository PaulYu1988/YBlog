namespace YBlog.Models.Custom
{
    public class UserInteractionView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public int ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
