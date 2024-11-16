using YBlog.Models.Custom;

namespace YBlog.Services
{
    public interface IStatisticService
    {
        DashboardView GetDashboardView();
        StatisticView GetStatisticView();
    }
}
