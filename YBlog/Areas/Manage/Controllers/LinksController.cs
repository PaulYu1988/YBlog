using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class LinksController : Controller
    {
        protected ILinkService _linkService;
        protected ICacheService _cacheService;
        protected IOperationService _operationService;
        public LinksController(ILinkService linkService, ICacheService cacheService, IOperationService operationService)
        {
            _linkService = linkService;
            _cacheService = cacheService;
            _operationService = operationService;
        }
        public async Task<IActionResult> IndexAsync(LinkPagedQuery query)
        {
            var items = await _linkService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {

            var link = new LinkRequest();
            if (id.HasValue)
            {
                var item = await _linkService.GetByIdAsync(id.Value);
                if (item != null)
                {
                    link.Id = item.Id;
                    link.Sort = item.Sort;
                    link.Text = item.Text;
                    link.Url = item.Url;
                    link.Target = item.Target;
                    link.IsEnabled = item.IsEnabled;
                }
            }
            return View(link);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(LinkRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var userState = Request.GetUserState();
            var result = false;
            if (request.Id.HasValue)
            {
                result = await _linkService.UpdateAsync(request);
                if (result)
                {
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.Link, request.Id, $"{request.Text} {request.Url}");
                }
            }
            else
            {
                var link = new Link()
                {
                    Text = request.Text ?? string.Empty,
                    Url = request.Url ?? string.Empty,
                    Target = request.Target ?? (int)EnumTargets._self,
                    Sort = request.Sort,
                    IsEnabled = request.IsEnabled
                };
                result = await _linkService.CreateAsync(link);
                if (result)
                {
                    request.Id = link.Id;
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Create, EnumOperationReferenceTypes.Link, request.Id, $"{request.Text} {request.Url}");
                }
            }
            _cacheService.Clear(EnumCacheNames.Links);
            return result ? this.Success(new { request.Id }) :
                this.InternalServerError();
        }
        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _linkService.GetByIdAsync(id);
            if (curr == null)
            {
                return this.NotFound();
            }
            var result = await _linkService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Link, id, $"{curr.Text} {curr.Url}");
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
