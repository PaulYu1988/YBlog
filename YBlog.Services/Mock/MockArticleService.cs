using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockArticleService : IArticleService
    {
        public Task<bool> CreateAsync(ArticleRequest request, int userId)
        {
            request.Id = 1;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(id % 2 == 0 ? true : false);
        }

        public Task<List<Article>> GetAllAsync()
        {
            return Task.FromResult(GetMockList());
        }

        public Task<List<Article>> GetAsync(ArticlePagedQuery query)
        {
            var items = new List<Article>();
            if (query.Title != "none") {
                items = GetMockList();
            }
            var totalCount = 250;
            query.TotalCount = totalCount;
            query.PageCount = totalCount % query.PageSize == 0 ? totalCount / query.PageSize : (totalCount / query.PageSize + 1);
            return Task.FromResult(items);
        }

        public Task<List<Article>> GetAsync(ArticleTagPagedQuery query)
        {
            var items = GetMockList();
            return Task.FromResult(items);
        }

        public Task<Article?> GetByIdAsync(int id)
        {
            var item = GetMockList().FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<List<Article>> GetRelatedAsync(int articleId, int take = 8)
        {
            var items = GetMockList().Take(take).ToList();
            return Task.FromResult(items);
        }

        public Task ReadAsync(Article article)
        {
            return Task.CompletedTask;
        }

        public Task<bool> UpdateAsync(ArticleRequest request)
        {
            return Task.FromResult(true);
        }

        private List<Article> GetMockList()
        {
            var items = new List<Article>();
            for (var i = 0; i < 20; i++)
            {
                var item = new Article();
                item.Id = i + 1;
                item.Title = "生活的美好生活的美好";
                item.ArticleContent = "<h2>h2标题介绍123321</h2><p>生活，是一幅五彩斑斓的画，充满了各种各样的色彩和情感。在这个世界上，每个人都在不同的生活轨迹上行进，经历着各种各样的风景和挑战。</p><p>有时，生活像一朵盛开的鲜花，散发着芬芳，让人感受到无限的喜悦和幸福。而有时，生活又像一场暴风雨，狂风骤雨中带来的是挫折和困难。</p><h2>h2标题介绍123321</h2><p>然而，正是这些各种各样的经历和挑战，让我们变得更加坚强和成熟。正是在生活的起伏中，我们学会了感恩和珍惜，学会了如何去面对困难和挑战。</p><ul><li>分析第一个原因</li><li>分析第二个原因</li><li>分析第三个原因</li></ul><h3>h3标题介绍123321</h3><p>所以，无论生活中遇到什么样的困难和挑战，我们都要学会勇敢地面对，相信自己的力量，相信明天会更美好。因为生活，就是一种奇妙的旅程，值得我们用心去体味和珍惜。</p><ol><li>分析第一个原因</li><li>分析第二个原因</li><li>分析第三个原因</li></ol><p><pre class=\"language-csharp\"><code>using System;</code></pre></p><p>所以，无论生活中遇到什么样的困难和挑战，我们都要学会勇敢地面对，相信自己的力量，相信明天会更美好。因为生活，就是一种奇妙的旅程，值得我们用心去体味和珍惜。</p>";
                item.Status = (int)EnumArticleStatuses.Publish;
                item.CreatedAt = DateTime.Now.AddDays(0 - i);
                item.Description = "生活，是一幅五彩斑斓的画，充满了各种各样的色彩和情感。在这个世界上，每个人都在不同的生活轨迹上行进，经历着各种各样的风景和挑战。";
                item.IsDeleted = false;
                item.UserId = i;
                item.CategoryId = 1;
                item.ReadCount = 100;
                item.CommentCount = 10;
                item.LastModifiedAt = DateTime.Now.AddHours(0 - i);
                items.Add(item);
            }
            return items;
        }
    }
}
