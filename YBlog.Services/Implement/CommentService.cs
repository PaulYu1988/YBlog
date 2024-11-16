using Microsoft.EntityFrameworkCore;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class CommentService : ICommentService
    {
        protected YBlogContext _context;
        protected IArticleService _articleService;
        public CommentService(YBlogContext context, IArticleService articleService)
        {
            _context = context;
            _articleService = articleService;
        }
        public async Task<bool> CreateAsync(CommentRequest request, int userId)
        {
            if (!request.ArticleId.HasValue)
            {
                return false;
            }
            var article = await _articleService.GetByIdAsync(request.ArticleId.Value);
            if (article == null)
                return false;
            var comment = new Comment()
            {
                ArticleId = article.Id,
                CommentContent = request.CommentContent ?? string.Empty,
                IsDeleted = false,
                Status = (int)EnumCommentStatuses.Approved,
                UserId = userId
            };
            _context.Comments.Add(comment);
            var date = DateTime.Now.Date;
            var commentDailyStatistic = await _context.DailyStatistics.FirstOrDefaultAsync(x => x.Type == (int)EnumDailyStatisticTypes.Comment && x.Date == date);
            if (commentDailyStatistic != null)
            {
                commentDailyStatistic.Count++;
                commentDailyStatistic.LastModifiedAt = DateTime.Now;
            }
            else
            {
                commentDailyStatistic = new DailyStatistic()
                {
                    Type = (int)EnumDailyStatisticTypes.Comment,
                    Date = date,
                    Count = 1
                };
                _context.DailyStatistics.Add(commentDailyStatistic);
            }
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var curr = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (curr == null)
                return false;
            curr.IsDeleted = true;
            curr.LastModifiedAt = DateTime.Now;
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<List<CommentView>> GetAsync(CommentPagedQuery query)
        {
            var result = new List<CommentView>();
            var queryable = _context.Comments.AsQueryable();
            if (query.ArticleId.HasValue)
            {
                queryable = queryable.Where(x => x.ArticleId == query.ArticleId);
            }
            if (query.UserId.HasValue)
            {
                queryable = queryable.Where(x => x.UserId == query.UserId);
            }
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(x => x.Status == query.Status);
            }
            var comments = await queryable.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToPagedListAsync(query);
            if (comments.Any())
            {
                var users = new List<User>();
                var userIds = comments.Where(x => x.UserId.HasValue).Select(x => x.UserId).Distinct().ToArray();
                if (userIds.Any())
                {
                    users = _context.Users.Where(x => userIds.Contains(x.Id)).ToList();
                }
                foreach (var comment in comments)
                {
                    var item = new CommentView()
                    {
                        ArticleId = comment.ArticleId,
                        CommentContent = comment.CommentContent,
                        CreatedAt = comment.CreatedAt,
                        Id = comment.Id
                    };
                    if (comment.UserId.HasValue)
                    {
                        var user = users.FirstOrDefault(x => x.Id == comment.UserId);
                        if (user != null)
                        {
                            item.Nickname = user.Nickname;
                            item.UserId = user.Id;
                            item.Avatar = user.Avatar;
                        }
                    }
                    result.Add(item);
                }
            }
            return result;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }
    }
}
