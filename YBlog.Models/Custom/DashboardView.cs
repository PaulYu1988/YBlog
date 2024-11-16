namespace YBlog.Models.Custom
{
    public class DashboardView
    {
        public int TodayRegisterCount { get; set; }
        public int TodayLoginCount { get; set; }
        public int ArticleCount { get; set; }
        public int CommentCount { get; set; }
        public int TagCount { get; set; }
        public string? ProcessName { get; set; }
        public long MemoryUsage { get; set; }
    }
}
