using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class HomeController : Controller
    {
        protected IArticleService _articleService;
        protected ICommentService _commentService;
        protected ICacheService _cacheService;
        protected ILogService _logService;
        protected IConfiguration _configuration;
        public HomeController(IArticleService articleService, ICommentService commentService, ICacheService cacheService, ILogService logService, IConfiguration configuration)
        {
            _articleService = articleService;
            _commentService = commentService;
            _cacheService = cacheService;
            _logService = logService;
            _configuration = configuration;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var installed = _configuration.GetValue<bool?>("Installed");
            if (installed != true)
            {
                return Redirect("/install");
            }
            var articleQuery = new ArticlePagedQuery()
            {
                Status = (int)EnumArticleStatuses.Publish
            };
            articleQuery.SetPageSize(10);
            ViewBag.HomeArticles = await _articleService.GetAsync(articleQuery);
            var commentQuery = new CommentPagedQuery()
            {
                Status = (int)EnumCommentStatuses.Approved
            };
            commentQuery.SetPageSize(6);
            ViewBag.CommentList = await _commentService.GetAsync(commentQuery);
            ViewBag.TopArticles = _cacheService.GetTopArticles();
            ViewBag.TopTags = _cacheService.GetTopTags();
            var webConfigs = _cacheService.GetWebConfigView();
            ViewData["Title"] = webConfigs.SiteName;
            ViewData["MetaKeywords"] = webConfigs.MetaKeywords;
            ViewData["MetaDescription"] = webConfigs.MetaDescription;
            return View();
        }
        public IActionResult Error(string? id)
        {
            var exceptionHandlerPathFeature = Request.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var message = exceptionHandlerPathFeature?.Error?.ToString();
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logService.Log(EnumLogTypes.Error, message);
            }
            if (Request.IsAjax())
            {
                switch (id)
                {
                    case "404":
                        return this.NotFound();
                    case "401":
                        return this.Unauthorized();
                    case "400":
                        return this.BadRequest();
                    default:
                        return this.InternalServerError();
                }
            }
            else
            {
                if (id == "404")
                {
                    return View("404");
                }
                return View();
            }
        }
    }
}
