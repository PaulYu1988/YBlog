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
            var currentProcess = Process.GetCurrentProcess();
            long memoryUsage = currentProcess.WorkingSet64;
            var dashboardView = new DashboardView()
            {
                MemoryUsage = memoryUsage / (1024 * 1024),
                ProcessName = currentProcess.ProcessName
            };
            dashboardView.TodayRegisterCount = _context.Users.Count(x => x.CreatedAt.Date == DateTime.Now.Date);
            dashboardView.TodayLoginCount = _context.Users.Count(x => x.LastLoginedAt.Date == DateTime.Now.Date);
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
            var commentDailyStatistics = _context.DailyStatistics.Where(x => x.Type == (int)EnumDailyStatisticTypes.Comment && x.Date >= preDay).ToList();
            for (var i = 1; i <= showDay; i++)
            {
                var date = preDay.AddDays(i);
                statistics.DailyStatistics.Add(new DailyStatisticView()
                {
                    Date = date.ToString("MM-dd"),
                    ReadCount = articleDailyStatistics.FirstOrDefault(x => x.Date == date)?.Count ?? 0,
                    CommentCount = commentDailyStatistics.FirstOrDefault(x => x.Date == date)?.Count ?? 0,
                });
            }
            return statistics;
        }
    }
}
