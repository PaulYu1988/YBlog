using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Common
{
    [Area("Common")]
    public class ReadCountController : Controller
    {
        protected IArticleService _articleService;
        public ReadCountController(IArticleService articleService)
        {
            _articleService = articleService;
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromBody] ReadCountRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            if (!request.Id.HasValue)
            {
                return this.BadRequest();
            }
            var isBot = Request.CheckIsBot();
            if (isBot)
                return this.BadRequest();
            var result = await _articleService.ReadAsync(request.Id.Value);
            return result ? this.Success() : this.BadRequest();
        }
    }
}
