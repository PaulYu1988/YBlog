using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class TagsController : Controller
    {
        protected ITagService _tagService;
        protected IOperationService _operationService;
        public TagsController(ITagService tagService, IOperationService operationService)
        {
            _tagService = tagService;
            _operationService = operationService;
        }
        public async Task<IActionResult> IndexAsync(TagPagedQuery query)
        {
            var items = await _tagService.GetAsync(query);
            ViewBag.Query = query;
            var tagCounts = await _tagService.GetTagCountsAsync(items.Select(x => x.Id).ToArray());
            ViewBag.TagCounts = tagCounts;
            return View(items);
        }
        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _tagService.GetByIdAsync(id);
            if (curr == null)
            {
                return this.NotFound();
            }
            var result = await _tagService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Tag, id, curr.Text);
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
