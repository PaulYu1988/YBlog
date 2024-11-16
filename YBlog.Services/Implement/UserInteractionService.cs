using Microsoft.EntityFrameworkCore;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class UserInteractionService : IUserInteractionService
    {
        protected YBlogContext _context;
        public UserInteractionService(YBlogContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(UserInteraction item)
        {
            _context.UserInteractions.Add(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await _context.UserInteractions.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }
            _context.UserInteractions.Remove(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Task<List<UserInteraction>> GetAsync(UserInteractionPagedQuery query)
        {
            var queryable = _context.UserInteractions.AsQueryable();
            if (query.ArticleId.HasValue)
            {
                queryable = queryable.Where(x => x.ArticleId == query.ArticleId);
            }
            if (query.UserId.HasValue)
            {
                queryable = queryable.Where(x => x.UserId == query.UserId);
            }
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(x => x.Type == query.Type);
            }
            var items = queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public async Task<ArticleUserInteraction> GetByArticleIdAsync(int articleId, int? userId)
        {
            var item = new ArticleUserInteraction();
            item.LikeCount = await _context.UserInteractions.CountAsync(x => x.ArticleId == articleId && x.Type == (int)EnumUserInteractionTypes.Like);
            item.FavoriteCount = await _context.UserInteractions.CountAsync(x => x.ArticleId == articleId && x.Type == (int)EnumUserInteractionTypes.Favorite);
            if (userId.HasValue)
            {
                item.LikeChecked = await _context.UserInteractions.AnyAsync(x => x.ArticleId == articleId && x.UserId == userId && x.Type == (int)EnumUserInteractionTypes.Like);
                item.FavoriteChecked = await _context.UserInteractions.AnyAsync(x => x.ArticleId == articleId && x.UserId == userId && x.Type == (int)EnumUserInteractionTypes.Favorite);
            }
            return item;
        }

        public Task<UserInteraction?> GetByIdAsync(int id)
        {
            return _context.UserInteractions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<UserInteractionView>> GetViewsAsync(UserInteractionViewPagedQuery query)
        {
            var queryable = from ui in _context.UserInteractions
                            join a in _context.Articles
                            on ui.ArticleId equals a.Id
                            select new UserInteractionView()
                            {
                                Id = ui.Id,
                                UserId = ui.UserId,
                                Type = ui.Type,
                                ArticleId = a.Id,
                                Title = a.Title,
                                CreatedAt = ui.CreatedAt
                            };
            if (query.UserId.HasValue)
            {
                queryable = queryable.Where(x => x.UserId == query.UserId);
            }
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(x => x.Type == query.Type);
            }
            if (!string.IsNullOrEmpty(query.Title))
            {
                queryable = queryable.Where(x => x.Title.Contains(query.Title));
            }
            var items = queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }
    }
}
