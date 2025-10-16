using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class ArticlesController : Controller
    {
        protected IArticleService _articleService;
        protected ICategoryService _categoryService;
        protected ICommentService _commentService;
        protected ITagService _tagService;
        protected IUserInteractionService _userInteractionService;
        public ArticlesController(IArticleService articleService, ICategoryService categoryService, ICommentService commentService, ITagService tagService, IUserInteractionService userInteractionService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _commentService = commentService;
            _tagService = tagService;
            _userInteractionService = userInteractionService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/");
            }
            var item = await _articleService.GetByIdAsync(id.Value);
            if (item == null || item.Status != (int)EnumArticleStatuses.Publish)
            {
                return Redirect("/");
            }
            await _articleService.ReadAsync(item);
            ViewBag.Category = await _categoryService.GetByIdAsync(item.CategoryId);
            HandleArticleContent(item);
            ViewBag.RelatedArticles = await _articleService.GetRelatedAsync(item.Id);
            ViewBag.Comments = await _commentService.GetAsync(new CommentPagedQuery()
            {
                ArticleId = item.Id,
                Status = (int)EnumCommentStatuses.Approved
            });
            var tags = await _tagService.GetArticleTagsAsync(item.Id);
            ViewData["Title"] = item.Title;
            if (tags.Any())
            {
                ViewBag.Tags = tags;
                ViewData["MetaKeywords"] = string.Join(",", tags.Select(x => x.Text).ToArray());
            }
            ViewData["MetaDescription"] = item.Description;
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> CommentsAsync(CommentRequest request)
        {
            var userState = Request.GetUserState();
            if (!userState.IsLogin)
            {
                return this.Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var captchaSessionName = EnumSessionNames.Captcha.ToString();
            var captcha = Request.HttpContext.Session.GetString(captchaSessionName);
            Request.HttpContext.Session.Remove(captchaSessionName);
            if (!string.Equals(captcha, request.Captcha, StringComparison.OrdinalIgnoreCase))
            {
                return this.Error(EnumErrorCodes.CaptchaError);
            }
            var result = await _commentService.CreateAsync(request, userState.Id);
            return result ? this.Success() : this.InternalServerError();
        }
        private void HandleArticleContent(Article item)
        {
            var document = new HtmlDocument();
            document.LoadHtml(item.ArticleContent);
            // Get summary.
            var nodes = document.DocumentNode.SelectNodes("//h2|//h3");
            var summary = new List<SummaryItem>();
            if (nodes != null && nodes.Count > 0)
            {
                SummaryItem? curr = null;
                HtmlNode? currNode = null;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    if (curr == null)
                    {
                        curr = new SummaryItem()
                        {
                            Id = "id" + i,
                            Text = node.InnerText
                        };
                        currNode = node;
                        node.Attributes.Add("id", curr.Id);
                        summary.Add(curr);
                    }
                    else
                    {
                        if (currNode?.Name == node.Name)
                        {
                            var newSummary = new SummaryItem()
                            {
                                Id = "id" + i,
                                Text = node.InnerText
                            };
                            summary.Add(newSummary);
                            curr = newSummary;
                            node.Attributes.Add("id", newSummary.Id);
                            currNode = node;
                        }
                        else
                        {
                            var child = new SummaryItem()
                            {
                                Id = "id" + i,
                                Text = node.InnerText
                            };
                            node.Attributes.Add("id", child.Id);
                            curr.Children.Add(child);
                        }
                    }
                }
                item.ArticleContent = document.DocumentNode.InnerHtml;
            }
            ViewBag.Summary = summary;

            // Set table class.
            var tableNodes = document.DocumentNode.SelectNodes("//table");
            if (tableNodes != null && tableNodes.Count > 0)
            {
                for (int i = 0; i < tableNodes.Count; i++)
                {
                    var tableNode = tableNodes[i];
                    tableNode.AddClass("layui-table");
                    tableNode.SetAttributeValue("lay-even", string.Empty);
                }
                item.ArticleContent = document.DocumentNode.InnerHtml;
            }
        }
        [HttpGet]
        public async Task<IActionResult> UserInteractionsAsync(int id)
        {
            var userState = Request.GetUserState();
            var item = await _userInteractionService.GetByArticleIdAsync(id, userState.IsLogin ? userState.Id : null);
            return this.Success(item);
        }
        [HttpPost]
        public async Task<IActionResult> UserInteractionsAsync(UserInteractionRequest request)
        {
            var userState = Request.GetUserState();
            if (!userState.IsLogin)
                return this.Unauthorized();
            if (!ModelState.IsValid)
                return this.BadRequest();
            if (!request.Type.HasValue || !request.ArticleId.HasValue)
                return this.BadRequest();
            var type = (int)request.Type.Value;
            var article = await _articleService.GetByIdAsync(request.ArticleId.Value);
            if (article == null)
                return this.BadRequest();
            var userInteractions = await _userInteractionService.GetAsync(new UserInteractionPagedQuery()
            {
                UserId = userState.Id,
                ArticleId = article.Id,
                Type = type
            });
            if (request.Checked)
            {
                if (!userInteractions.Any())
                {
                    var item = new UserInteraction()
                    {
                        UserId = userState.Id,
                        ArticleId = article.Id,
                        Type = type
                    };
                    await _userInteractionService.CreateAsync(item);
                }
            }
            else
            {
                if (userInteractions.Any())
                {
                    foreach (var item in userInteractions)
                    {
                        await _userInteractionService.DeleteByIdAsync(item.Id);
                    }
                }
            }
            return this.Success();
        }
    }
}
