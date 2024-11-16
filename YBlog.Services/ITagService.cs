using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;

namespace YBlog.Services
{
    public interface ITagService
    {
        Task<List<Tag>> GetArticleTagsAsync(int articleId);
        Task<List<Tag>> GetAsync(TagPagedQuery query);
        Task<List<Tag>> GetAllAsync();
        Task CreateArticleTagsAsync(string tags, int articleId);
        Task UpdateArticleTagsAsync(string tags, int articleId);
        Task<Tag?> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<List<TagCount>> GetTagCountsAsync(int[] tagIds);
    }
}
