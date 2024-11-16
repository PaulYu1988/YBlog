using Microsoft.AspNetCore.Mvc;
using YBlog.Models.Queries;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class TagsController : Controller
    {
        protected IArticleService _articleService;
        protected ITagService _tagService;
        protected ICacheService _cacheService;
        public TagsController(IArticleService articleService, ITagService tagService, ICacheService cacheService)
        {
            _articleService = articleService;
            _tagService = tagService;
            _cacheService = cacheService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync(int? id, int? page)
        {
            if (!id.HasValue)
            {
                return Redirect("/");
            }
            var tag = await _tagService.GetByIdAsync(id.Value);
            if (tag == null)
            {
                return Redirect("/");
            }
            var query = new ArticleTagPagedQuery()
            {
                Page = page,
                TagId = id.Value
            };
            var items = await _articleService.GetAsync(query);
            ViewBag.Articles = items;
            ViewBag.Query = query;
            ViewBag.Tag = tag;
            ViewBag.TopTags = _cacheService.GetTopTags();
            return View();
        }
    }
}
