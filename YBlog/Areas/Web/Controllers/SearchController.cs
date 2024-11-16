using Microsoft.AspNetCore.Mvc;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class SearchController : Controller
    {
        protected IArticleService _articleService;
        protected ICacheService _cacheService;
        public SearchController(IArticleService articleService, ICacheService cacheService)
        {
            _articleService = articleService;
            _cacheService = cacheService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync(string? id, int? page)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Redirect("/");
            }
            var query = new ArticlePagedQuery()
            {
                Page = page,
                Title = id
            };
            var items = await _articleService.GetAsync(query);
            ViewBag.Articles = items;
            ViewBag.Query = query;
            ViewBag.TopArticles = _cacheService.GetTopArticles();
            return View();
        }
    }
}
