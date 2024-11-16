using Microsoft.EntityFrameworkCore;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class TagService : ITagService
    {
        protected YBlogContext _context;
        public TagService(YBlogContext context)
        {
            _context = context;
        }
        public async Task CreateArticleTagsAsync(string tags, int articleId)
        {
            var tagList = tags.Split(',');
            var currTags = await GenerateTagListAsync(tags);
            var sort = 0;
            foreach (var item in tagList)
            {
                sort++;
                var tag = currTags.FirstOrDefault(x => x.Text == item);
                if (tag != null)
                {
                    _context.ArticleTags.Add(new ArticleTag()
                    {
                        ArticleId = articleId,
                        TagId = tag.Id,
                        Sort = sort
                    });
                }
            }
            await _context.SaveChangesAsync();
        }
        private async Task<List<Tag>> GenerateTagListAsync(string tags)
        {
            var tagList = new string[0];
            if (!string.IsNullOrWhiteSpace(tags))
            {
                tagList = tags.Split(',');
            }
            var currTags = await _context.Tags.Where(x => tagList.Contains(x.Text)).ToListAsync();
            var addTags = new List<Tag>();
            foreach (var item in tagList)
            {
                if (!currTags.Any(x => x.Text == item))
                {
                    addTags.Add(new Tag()
                    {
                        Text = item
                    });
                }
            }
            await _context.Tags.AddRangeAsync(addTags);
            await _context.SaveChangesAsync();
            currTags.AddRange(addTags);
            return currTags;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<List<Tag>> GetArticleTagsAsync(int articleId)
        {
            return await (from t1 in _context.Tags
                          join t2 in _context.ArticleTags
                          on t1.Id equals t2.TagId
                          where t2.ArticleId == articleId
                          orderby t2.Sort
                          select t1).ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateArticleTagsAsync(string tags, int articleId)
        {
            var tagList = new string[0];
            if (!string.IsNullOrWhiteSpace(tags))
            {
                tagList = tags.Split(',');
            }
            var currTags = await GenerateTagListAsync(tags);
            var articleTags = await _context.ArticleTags.Where(x => x.ArticleId == articleId).ToListAsync();
            var currTagIds = currTags.Select(x => x.Id).ToArray();
            var deleteArticleTags = articleTags.Where(x => !currTagIds.Contains(x.TagId)).ToList();
            if (tagList.Any())
            {
                var sort = 0;
                foreach (var item in tagList)
                {
                    sort++;
                    var tag = currTags.FirstOrDefault(x => x.Text == item);
                    if (tag != null)
                    {
                        var currArticleTag = articleTags.FirstOrDefault(x => x.TagId == tag.Id);
                        if (currArticleTag != null)
                        {
                            currArticleTag.Sort = sort;
                            currArticleTag.LastModifiedAt = DateTime.Now;
                        }
                        else
                        {
                            var articleTag = new ArticleTag()
                            {
                                ArticleId = articleId,
                                TagId = tag.Id,
                                Sort = sort
                            };
                            await _context.ArticleTags.AddAsync(articleTag);
                        }
                    }
                }
            }
            if (deleteArticleTags.Any())
            {
                _context.ArticleTags.RemoveRange(deleteArticleTags);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAsync(TagPagedQuery query)
        {
            var queryable = _context.Tags.AsQueryable();
            if (!string.IsNullOrEmpty(query.Text))
            {
                queryable = queryable.Where(x => x.Text.Contains(query.Text));
            }
            var items = await queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var result = false;
            var curr = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (curr != null)
            {
                var articleTags = await _context.ArticleTags.Where(x => x.TagId == id).ToListAsync();
                if (articleTags.Any())
                {
                    _context.ArticleTags.RemoveRange(articleTags);
                }
                _context.Tags.Remove(curr);
                result = (await _context.SaveChangesAsync()) > 0;
            }
            return result;
        }

        public Task<List<TagCount>> GetTagCountsAsync(int[] tagIds)
        {
            return _context.ArticleTags.Where(x => tagIds.Contains(x.TagId)).GroupBy(x => x.TagId).Select(x => new TagCount() { TagId = x.Key, Count = x.Count() }).ToListAsync();
        }
    }
}
