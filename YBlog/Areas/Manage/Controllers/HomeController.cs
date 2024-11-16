using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Services;
using YBlog.Models.Queries;
using YBlog.Extensions;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class HomeController : Controller
    {
        protected IOperationService _operationService;
        protected ILogService _logService;
        protected IStatisticService _statisticService;
        public HomeController(IOperationService operationService, ILogService logService, IStatisticService statisticService)
        {
            _operationService = operationService;
            _logService = logService;
            _statisticService = statisticService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var dashboardView = _statisticService.GetDashboardView();
            var operationQuery = new OperationPagedQuery();
            operationQuery.SetPageSize(8);
            var items = await _operationService.GetAsync(operationQuery);
            ViewBag.Operations = await _operationService.GetViewsAsync(items);
            var logQuery = new LogPagedQuery();
            logQuery.SetPageSize(8);
            ViewBag.Logs = await _logService.GetAsync(logQuery);
            return View(dashboardView);
        }
        [HttpGet]
        public IActionResult Statistics()
        {
            var statistics = _statisticService.GetStatisticView();
            return this.Success(statistics);
        }
    }
}
