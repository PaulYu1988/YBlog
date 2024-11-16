using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class ArticleService : IArticleService
    {
        protected YBlogContext _context;
        protected ICategoryService _categoryService;
        public ArticleService(YBlogContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task<bool> CreateAsync(ArticleRequest request, int userId)
        {
            if (!request.CategoryId.HasValue)
                return false;
            var category = await _categoryService.GetByIdAsync(request.CategoryId.Value);
            if (category == null)
                return false;
            var article = new Article()
            {
                Title = request.Title ?? string.Empty,
                Description = request.Description,
                Status = (int)(request.Status ?? EnumArticleStatuses.Draft),
                ArticleContent = request.ArticleContent ?? string.Empty,
                CategoryId = request.CategoryId.Value,
                Thumbnail = request.Thumbnail,
                UserId = userId,
                IsDeleted = false,
                IsTop = request.IsTop
            };
            _context.Articles.Add(article);
            var result = (await _context.SaveChangesAsync()) > 0;
            if (result)
            {
                request.Id = article.Id;
            }
            return result;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }
            item.IsDeleted = true;
            item.LastModifiedAt = DateTime.Now;
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Task<List<Article>> GetAllAsync()
        {
            return _context.Articles.Where(x => x.Status == (int)EnumArticleStatuses.Publish && x.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Article>> GetAsync(ArticlePagedQuery query)
        {
            var queryable = _context.Articles.AsQueryable();
            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(x => x.CategoryId == query.CategoryId);
            }
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                queryable = queryable.Where(x => x.Title.Contains(query.Title));
            }
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(x => x.Status == query.Status);
            }
            queryable = queryable.Where(x => x.IsDeleted == false);
            var items = await queryable.OrderByDescending(x => x.IsTop).ThenByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public async Task<List<Article>> GetAsync(ArticleTagPagedQuery query)
        {
            var queryable = from at in _context.ArticleTags
                            join a in _context.Articles
                            on at.ArticleId equals a.Id
                            select new
                            {
                                ArticleTags = at,
                                Article = a
                            };
            if (query.TagId.HasValue)
            {
                queryable = queryable.Where(x => x.ArticleTags.TagId == query.TagId);
            }
            var items = await queryable.OrderByDescending(x => x.ArticleTags.Id).Select(x => x.Article).ToPagedListAsync(query);
            return items;
        }

        public Task<Article?> GetByIdAsync(int id)
        {
            return _context.Articles.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }

        public async Task<List<Article>> GetRelatedAsync(int articleId, int take = 8)
        {
            var result = new List<Article>();
            var relatedArticles = await (
                from a in _context.Articles
                join at1 in _context.ArticleTags on a.Id equals at1.ArticleId
                join at2 in _context.ArticleTags on at1.TagId equals at2.TagId
                where a.Id != articleId && a.Status == 2 && at2.ArticleId == articleId
                orderby Guid.NewGuid()
                select a
            ).Take(take).ToListAsync();
            result.AddRange(relatedArticles);
            int len = result.Count();
            if (len < take)
            {
                var ids = result.Select(x => x.Id).ToList();
                ids.Add(articleId);
                var others = await _context.Articles.Where(x => x.Status == 2 && x.IsDeleted == false && !ids.Contains(x.Id)).OrderBy(x => Guid.NewGuid()).Take(take - len).ToListAsync();
                result.AddRange(others);
            }
            return result;
        }

        public async Task ReadAsync(Article article)
        {
            article.ReadCount++;
            var date = DateTime.Now.Date;
            var articleDailyStatistic = await _context.DailyStatistics.FirstOrDefaultAsync(x => x.Type == (int)EnumDailyStatisticTypes.Article && x.Date == date);
            if (articleDailyStatistic != null)
            {
                articleDailyStatistic.Count++;
                articleDailyStatistic.LastModifiedAt = DateTime.Now;
            }
            else
            {
                articleDailyStatistic = new DailyStatistic()
                {
                    Type = (int)EnumDailyStatisticTypes.Article,
                    Date = date,
                    Count = 1
                };
                _context.DailyStatistics.Add(articleDailyStatistic);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(ArticleRequest request)
        {
            if (!request.Id.HasValue)
                return false;
            if (!request.CategoryId.HasValue)
                return false;
            var category = await _categoryService.GetByIdAsync(request.CategoryId.Value);
            if (category == null)
                return false;
            var curr = await GetByIdAsync(request.Id.Value);
            if (curr == null)
                return false;
            curr.Title = request.Title ?? string.Empty;
            curr.Description = request.Description;
            curr.Status = (int)(request.Status ?? EnumArticleStatuses.Draft);
            curr.ArticleContent = request.ArticleContent ?? string.Empty;
            curr.CategoryId = request.CategoryId.Value;
            curr.Thumbnail = request.Thumbnail;
            curr.LastModifiedAt = DateTime.Now;
            curr.IsTop = request.IsTop;
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
