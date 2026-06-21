using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Services;
using YBlog.Models.Queries;
using YBlog.Extensions;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class HomeController : Controller
    {
        protected IOperationService _operationService;
        protected ILogService _logService;
        protected IStatisticService _statisticService;
        protected ICommentService _commentService;
        protected IArticleService _articleService;
        protected IUserService _userService;
        public HomeController(IOperationService operationService, ILogService logService, IStatisticService statisticService, ICommentService commentService, IArticleService articleService, IUserService userService)
        {
            _operationService = operationService;
            _logService = logService;
            _statisticService = statisticService;
            _commentService = commentService;
            _articleService = articleService;
            _userService = userService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var dashboardView = _statisticService.GetDashboardView();
            var operationQuery = new OperationPagedQuery();
            operationQuery.SetPageSize(5);
            var items = await _operationService.GetAsync(operationQuery);
            ViewBag.Operations = await _operationService.GetViewsAsync(items);
            var articleQuery = new ArticlePagedQuery();
            articleQuery.SetPageSize(5);
            ViewBag.ArticleList = await _articleService.GetAsync(articleQuery);
            var commentQuery = new CommentPagedQuery();
            commentQuery.SetPageSize(5);
            ViewBag.CommentList = await _commentService.GetAsync(commentQuery);
            var userQuery = new UserPagedQuery();
            userQuery.SetPageSize(5);
            var userList = await _userService.GetAsync(userQuery);
            ViewBag.UserList = userList;
            ViewBag.UserCredentials = await _userService.GetUserCredentialsByUserIds(userList.Select(x => x.Id).ToArray());
            var logQuery = new LogPagedQuery();
            logQuery.SetPageSize(3);
            ViewBag.Logs = await _logService.GetAsync(logQuery);
            return View(dashboardView);
        }
        [HttpGet]
        public IActionResult Statistics()
        {
            var statistics = _statisticService.GetStatisticView();
            return this.Success(statistics);
        }
    }
}
