using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;

namespace YBlog.Services.Mock
{
    public class MockTagService : ITagService
    {
        public Task CreateArticleTagsAsync(string tags, int articleId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(id % 2 == 0 ? true : false);
        }

        public Task<List<Tag>> GetAllAsync()
        {
            var items = GetMockList();
            return Task.FromResult(items);
        }

        public Task<List<Tag>> GetArticleTagsAsync(int articleId)
        {
            var items = GetMockList().Take(3).ToList();
            return Task.FromResult(items);
        }

        public Task<List<Tag>> GetAsync(TagPagedQuery query)
        {
            var items = new List<Tag>();
            if (query.Text != "none") {
                items.Add(new Tag()
                {
                    Id = 1,
                    Text = "Tag1"
                });
                items.Add(new Tag()
                {
                    Id = 2,
                    Text = "Tag2"
                });
            }
            return Task.FromResult(items);
        }

        public Task<Tag?> GetByIdAsync(int id)
        {
            var item = GetMockList().FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task UpdateArticleTagsAsync(string tags, int articleId)
        {
            return Task.FromResult(true);
        }

        public Task<List<TagCount>> GetTagCountsAsync(int[] tagIds)
        {
            var result = new List<TagCount>();
            foreach (var tagId in tagIds)
            {
                var item = new TagCount()
                {
                    TagId = tagId,
                    Count = new Random(tagId).Next(0, 12)
                };
                result.Add(item);
            }
            return Task.FromResult(result);
        }

        private List<Tag> GetMockList()
        {
            var items = new List<Tag>();
            for (var i = 0; i < 20; i++)
            {
                var tag = new Tag()
                {
                    Text = "标签" + new Random(i).Next(0, 1000),
                    Id = i + 1,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now
                };
                items.Add(tag);
            }
            return items;
        }
    }
}
