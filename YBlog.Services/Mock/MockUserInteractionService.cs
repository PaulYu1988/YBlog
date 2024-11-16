using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;

namespace YBlog.Services.Mock
{
    public class MockUserInteractionService : IUserInteractionService
    {
        public Task<bool> CreateAsync(UserInteraction item)
        {
            Task.Delay(300);
            return Task.FromResult(DateTime.Now.Ticks % 2 == 0);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            Task.Delay(300);
            return Task.FromResult(DateTime.Now.Ticks % 2 == 0);
        }

        public Task<List<UserInteraction>> GetAsync(UserInteractionPagedQuery query)
        {
            var items = new List<UserInteraction>();
            return Task.FromResult(items);
        }

        public Task<ArticleUserInteraction> GetByArticleIdAsync(int articleId, int? userId)
        {
            var item = new ArticleUserInteraction()
            {
                FavoriteChecked = true,
                FavoriteCount = 1010,
                LikeChecked = false,
                LikeCount = 20100
            };
            return Task.FromResult(item);
        }

        public Task<UserInteraction?> GetByIdAsync(int id)
        {
            var items = new List<UserInteraction>();
            items.Add(new UserInteraction()
            {
                Id = id,
                UserId = 1,
                ArticleId = 1
            });
            var item = items.FirstOrDefault();
            return Task.FromResult(item);
        }

        public Task<List<UserInteractionView>> GetViewsAsync(UserInteractionViewPagedQuery query)
        {
            var items = new List<UserInteractionView>();
            for (int i = 0; i < query.PageSize; i++)
            {
                var id = i + 1;
                items.Add(new UserInteractionView()
                {
                    Id = id,
                    UserId = 1,
                    ArticleId = id,
                    Title = "这是第" + id + "篇文章的标题",
                    Type = query.Type ?? 0,
                    CreatedAt = DateTime.Now
                });
            }
            return Task.FromResult(items);
        }
    }
}
