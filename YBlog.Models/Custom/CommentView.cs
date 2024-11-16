namespace YBlog.Models.Custom
{
    public class CommentView
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string? CommentContent { get; set; }
        public int? UserId { get; set; }
        public string? Nickname { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
