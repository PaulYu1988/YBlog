using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Account.Controllers
{
    [Area("Account")]
    [PostLogin]
    public class CommentsController : Controller
    {
        protected ICommentService _commentService;
        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        public async Task<IActionResult> IndexAsync(CommentPagedQuery query)
        {
            var userState = Request.GetUserState();
            query.UserId = userState.Id;
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
            var userState = Request.GetUserState();
            if (curr.UserId != userState.Id)
            {
                return this.NotFound();
            }
            var result = await _commentService.DeleteByIdAsync(id);
            if (result)
            {
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
