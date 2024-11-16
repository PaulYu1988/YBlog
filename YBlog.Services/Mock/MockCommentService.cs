using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockCommentService : ICommentService
    {
        public Task<bool> CreateAsync(CommentRequest request, int userId)
        {
            return Task.FromResult((request.CommentContent ?? string.Empty).StartsWith("a") ? true : false);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(id % 2 == 0 ? true : false);
        }

        public Task<List<CommentView>> GetAsync(CommentPagedQuery query)
        {
            var items = GetViewList();
            return Task.FromResult(items);
        }

        public Task<Comment?> GetByIdAsync(int id)
        {
            var items = GetList();
            var item = items.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }
        private List<Comment> GetList()
        {
            var items = new List<Comment>();
            items.Add(new Comment()
            {
                Id = 1,
                ArticleId = 1,
                UserId = 1
            });
            return items;
        }
        private List<CommentView> GetViewList()
        {
            var items = new List<CommentView>();
            items.Add(new CommentView()
            {
                Id = 1,
                CommentContent = "Ha ha",
                Nickname = "Nickname",
                ArticleId = 1,
            });
            items.Add(new CommentView()
            {
                Id = 2,
                CommentContent = "k aka",
                ArticleId = 2,
            });
            return items;
        }
    }
}
