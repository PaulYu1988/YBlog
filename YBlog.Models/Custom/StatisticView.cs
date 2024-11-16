namespace YBlog.Models.Custom
{
    public class StatisticView
    {
        public List<ArticleStatisticView> ArticleStatistics { get; set; } = new List<ArticleStatisticView>();
        public List<DailyStatisticView> DailyStatistics { get; set; } = new List<DailyStatisticView>();
    }
    public class ArticleStatisticView
    {
        public string? CategoryName { get; set; }
        public int ArticleCount { get; set; }
    }
    public class DailyStatisticView
    {
        public string? Date { get; set; }
        public int ReadCount { get; set; }
        public int CommentCount { get; set; }
    }
}
