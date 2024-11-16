using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface IArticleService
    {
        Task<List<Article>> GetAsync(ArticlePagedQuery query);
        Task<Article?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ArticleRequest request, int userId);
        Task<bool> DeleteByIdAsync(int id);
        Task<bool> UpdateAsync(ArticleRequest request);
        Task ReadAsync(Article article);
        Task<List<Article>> GetRelatedAsync(int articleId, int take = 8);
        Task<List<Article>> GetAsync(ArticleTagPagedQuery query);
        Task<List<Article>> GetAllAsync();
    }
}
