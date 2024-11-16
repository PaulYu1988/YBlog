using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class OperationsController : Controller
    {
        protected IOperationService _operationService;
        public OperationsController(IOperationService operationService)
        {
            _operationService = operationService;
        }
        public async Task<IActionResult> IndexAsync(OperationPagedQuery query)
        {
            var items = await _operationService.GetAsync(query);
            var operations = await _operationService.GetViewsAsync(items);
            ViewBag.Query = query;
            return View(operations);
        }
    }
}
