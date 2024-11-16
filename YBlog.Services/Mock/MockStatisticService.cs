using System.Diagnostics;
using YBlog.Models.Custom;

namespace YBlog.Services.Mock
{
    public class MockStatisticService : IStatisticService
    {
        public DashboardView GetDashboardView()
        {
            var currentProcess = Process.GetCurrentProcess();
            long memoryUsage = currentProcess.WorkingSet64;
            var dashboardView = new DashboardView()
            {
                MemoryUsage = memoryUsage / (1024 * 1024),
                ProcessName = currentProcess.ProcessName
            };
            dashboardView.ArticleCount = 1000;

            return dashboardView;
        }

        public StatisticView GetStatisticView()
        {
            var statisticView = new StatisticView();
            statisticView.ArticleStatistics.Add(new ArticleStatisticView()
            {
                CategoryName = ".NET",
                ArticleCount = 50
            });
            statisticView.ArticleStatistics.Add(new ArticleStatisticView()
            {
                CategoryName = "Database",
                ArticleCount = 30
            });
            statisticView.ArticleStatistics.Add(new ArticleStatisticView()
            {
                CategoryName = "Angular",
                ArticleCount = 35
            });
            int showDay = 14;
            var preDay = DateTime.Now.AddDays(0 - showDay).Date;
            for (var i = 1; i <= showDay; i++)
            {
                var date = preDay.AddDays(i);
                var rnd = new Random(DateTime.Now.Second * i);
                statisticView.DailyStatistics.Add(new DailyStatisticView()
                {
                    Date = date.ToString("MM-dd"),
                    CommentCount = rnd.Next(10, 100),
                    ReadCount = rnd.Next(10, 100),
                });
            }
            return statisticView;
        }
    }
}
