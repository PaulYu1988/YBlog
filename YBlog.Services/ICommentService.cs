using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface ICommentService
    {
        Task<List<CommentView>> GetAsync(CommentPagedQuery query);
        Task<Comment?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CommentRequest request, int userId);
        Task<bool> DeleteByIdAsync(int id);
    }
}
