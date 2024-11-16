using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class NavsController : Controller
    {
        protected INavService _navService;
        protected ICacheService _cacheService;
        protected ICategoryService _categoryService;
        protected IOperationService _operationService;
        public NavsController(INavService navService, ICacheService cacheService, ICategoryService categoryService, IOperationService operationService)
        {
            _navService = navService;
            _cacheService = cacheService;
            _categoryService = categoryService;
            _operationService = operationService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var items = await _navService.GetAllAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id, int? parentId)
        {
            var nav = new NavRequest();
            ViewBag.Categories = await _categoryService.GetAllAsync();
            if (id.HasValue)
            {
                var item = await _navService.GetByIdAsync(id.Value);
                if (item != null)
                {
                    nav.Id = item.Id;
                    nav.Sort = item.Sort;
                    nav.Text = item.Text;
                    nav.Url = item.Url;
                    nav.Target = item.Target;
                    nav.IsEnabled = item.IsEnabled;
                    nav.ParentId = item.ParentId;
                    parentId = item.ParentId;
                }
            }
            if (parentId.HasValue)
            {
                var parent = await _navService.GetByIdAsync(parentId.Value);
                ViewBag.Parent = parent;
            }
            return View(nav);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(NavRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var userState = Request.GetUserState();
            var result = false;
            if (request.Id.HasValue)
            {
                result = await _navService.UpdateAsync(request);
                if (result)
                {
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.Nav, request.Id, $"{request.Text} {request.Url}");
                }
            }
            else
            {
                var nav = new Nav()
                {
                    Text = request.Text ?? string.Empty,
                    Url = request.Url ?? string.Empty,
                    Target = request.Target ?? (int)EnumTargets._self,
                    Sort = request.Sort,
                    IsEnabled = request.IsEnabled,
                    ParentId = request.ParentId
                };
                result = await _navService.CreateAsync(nav);
                if (result)
                {
                    request.Id = nav.Id;
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Create, EnumOperationReferenceTypes.Nav, request.Id, $"{request.Text} {request.Url}");
                }
            }
            _cacheService.Clear(EnumCacheNames.Navs);
            return result ? this.Success(new { request.Id }) :
                this.InternalServerError();
        }
        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _navService.GetByIdAsync(id);
            if (curr == null)
            {
                return this.NotFound();
            }
            var result = await _navService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Nav, id, $"{curr.Text} {curr.Url}");
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
