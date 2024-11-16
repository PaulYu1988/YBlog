namespace YBlog.Models.Custom
{
    public class SummaryItem
    {
        public string? Id { get; set; }
        public string? Text { get; set; } = null!;
        public List<SummaryItem> Children = new List<SummaryItem>();
    }
}
