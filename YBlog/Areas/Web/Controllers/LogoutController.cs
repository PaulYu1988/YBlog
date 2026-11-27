using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;
using YBlog.Models.Enums;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class LogoutController : Controller
    {
        protected IUserService _userService;
        public LogoutController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var userState = Request.GetUserState();
            if (userState.IsLogin && userState.Id > 0)
            {
                await _userService.RevokeTokenAsync(userState.Id);
            }
            Request.HttpContext.Session.Clear();
            Request.HttpContext.Response.Cookies.Delete(EnumCookieNames.Token.ToString());
            var headers = Request.GetTypedHeaders();
            if (headers != null)
            {
                var referer = headers.Referer;
                if (referer != null)
                {
                    return Redirect(referer.ToString());
                }
            }
            return Redirect("/");
        }
    }
}
