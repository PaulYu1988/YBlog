using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Queries;
using YBlog.Services;
using YBlog.Models.Enums;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class CommentsController : Controller
    {
        protected ICommentService _commentService;
        protected IOperationService _operationService;
        public CommentsController(ICommentService commentService, IOperationService operationService)
        {
            _commentService = commentService;
            _operationService = operationService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync(CommentPagedQuery query)
        {
            var items = await _commentService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _commentService.GetByIdAsync(id);
            if (curr == null)
            {
                return this.NotFound();
            }
            var result = await _commentService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Comment, id, curr.CommentContent);
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
