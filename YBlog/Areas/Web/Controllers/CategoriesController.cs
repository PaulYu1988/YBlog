using Microsoft.AspNetCore.Mvc;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class CategoriesController : Controller
    {
        protected IArticleService _articleService;
        protected ICategoryService _categoryService;
        protected ICacheService _cacheService;
        public CategoriesController(IArticleService articleService, ICategoryService categoryService, ICacheService cacheService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> IndexAsync(int? id, int? page)
        {
            if (!id.HasValue)
            {
                return Redirect("/");
            }
            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null)
            {
                return Redirect("/");
            }
            ViewBag.Category = category;
            var query = new ArticlePagedQuery()
            {
                Page = page,
                CategoryId = id
            };
            var items = await _articleService.GetAsync(query);
            ViewBag.Articles = items;
            ViewBag.Query = query;
            ViewBag.TopArticles = _cacheService.GetTopArticles();
            return View();
        }
    }
}
