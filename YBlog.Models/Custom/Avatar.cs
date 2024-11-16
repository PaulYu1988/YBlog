namespace YBlog.Models.Custom
{
    public class Avatar
    {
        public string? Url { get; set; }
        public int? UserId { get; set; }
        public string? Nickname { get; set; }
        public bool IsHeader { get; set; }
    }
}