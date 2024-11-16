using YBlog.Models.Database;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface INavService
    {
        Task<List<Nav>> GetAllAsync();
        Task<Nav?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(NavRequest request);
        Task<bool> CreateAsync(Nav item);
        Task<bool> DeleteByIdAsync(int id);
    }
}
