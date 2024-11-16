using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class LogsController : Controller
    {
        protected ILogService _logService;
        public LogsController(ILogService logService) { 
            _logService = logService;
        }
        public async Task<IActionResult> IndexAsync(LogPagedQuery query)
        {
            var items = await _logService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
    }
}
