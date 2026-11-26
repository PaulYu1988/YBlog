using System.Diagnostics;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;

namespace YBlog.Services.Implement
{
    public class StatisticService : IStatisticService
    {
        protected YBlogContext _context;
        public StatisticService(YBlogContext context)
        {
            _context = context;
        }
        public DashboardView GetDashboardView()
        {
            DashboardView dashboardView;
            using (var currentProcess = Process.GetCurrentProcess())
            {
                dashboardView = new DashboardView()
                {
                    MemoryUsage = currentProcess.WorkingSet64 / (1024 * 1024),
                    ProcessName = currentProcess.ProcessName
                };
            }
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            dashboardView.TodayRegisterCount = _context.Users.Count(x => x.CreatedAt >= today && x.CreatedAt < tomorrow);
            dashboardView.TodayLoginCount = _context.Users.Count(x => x.LastLoginedAt >= today && x.LastLoginedAt < tomorrow);
            dashboardView.UserCount = _context.Users.Count();
            dashboardView.ArticleCount = _context.Articles.Count(x => x.IsDeleted == false);
            dashboardView.CommentCount = _context.Comments.Count(x => x.IsDeleted == false);
            dashboardView.TagCount = _context.Tags.Count();
            return dashboardView;
        }

        public StatisticView GetStatisticView()
        {
            var statistics = new StatisticView();
            var result = from articles in _context.Articles
                         where !articles.IsDeleted
                         group articles by articles.CategoryId into g
                         select new
                         {
                             CategoryId = g.Key,
                             Count = g.Count()
                         };
            var categories = _context.Categories.ToList();
            foreach (var item in result)
            {
                var category = categories.FirstOrDefault(x => x.Id == item.CategoryId);
                if (category != null)
                {
                    statistics.ArticleStatistics.Add(new ArticleStatisticView()
                    {
                        CategoryName = category.Name,
                        ArticleCount = item.Count
                    });
                }
            }
            int showDay = 14;
            var preDay = DateTime.Now.AddDays(0 - showDay).Date;
            var articleDailyStatistics = _context.DailyStatistics.Where(x => x.Type == (int)EnumDailyStatisticTypes.Article && x.Date >= preDay).ToList();
            var articleBotDailyStatistics = _context.DailyStatistics.Where(x => x.Type == (int)EnumDailyStatisticTypes.ArticleBot && x.Date >= preDay).ToList();
            for (var i = 1; i <= showDay; i++)
            {
                var date = preDay.AddDays(i);
                statistics.DailyStatistics.Add(new DailyStatisticView()
                {
                    Date = date.ToString("MM-dd"),
                    ReadCount = articleDailyStatistics.FirstOrDefault(x => x.Date == date)?.Count ?? 0,
                    ArticleBotCount = articleBotDailyStatistics.FirstOrDefault(x => x.Date == date)?.Count ?? 0,
                });
            }
            return statistics;
        }
    }
}
