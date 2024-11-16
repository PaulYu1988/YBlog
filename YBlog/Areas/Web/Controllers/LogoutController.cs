using Microsoft.AspNetCore.Mvc;
using YBlog.Models.Enums;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
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
