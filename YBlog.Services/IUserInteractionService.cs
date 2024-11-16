using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;

namespace YBlog.Services
{
    public interface IUserInteractionService
    {
        Task<List<UserInteraction>> GetAsync(UserInteractionPagedQuery query);
        Task<UserInteraction?> GetByIdAsync(int id);
        Task<List<UserInteractionView>> GetViewsAsync(UserInteractionViewPagedQuery query);
        Task<ArticleUserInteraction> GetByArticleIdAsync(int articleId, int? userId);
        Task<bool> CreateAsync(UserInteraction item);
        Task<bool> DeleteByIdAsync(int id);
    }
}
