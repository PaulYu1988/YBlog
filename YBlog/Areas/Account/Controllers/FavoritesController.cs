using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Account.Controllers
{
    [Area("Account")]
    [PostLogin]
    public class FavoritesController : Controller
    {
        protected IUserInteractionService _userInteractionService;
        public FavoritesController(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }
        public async Task<IActionResult> IndexAsync(UserInteractionViewPagedQuery query)
        {
            var userState = Request.GetUserState();
            query.UserId = userState.Id;
            query.Type = (int)EnumUserInteractionTypes.Favorite;
            var items = await _userInteractionService.GetViewsAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _userInteractionService.GetByIdAsync(id);
            if (curr == null)
                return this.NotFound();
            var userState = Request.GetUserState();
            if (curr.UserId != userState.Id)
                return this.NotFound();
            var result = await _userInteractionService.DeleteByIdAsync(curr.Id);
            return result ? this.Success() : this.InternalServerError();
        }
    }
}
