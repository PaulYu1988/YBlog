using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAsync(CategoryPagedQuery query);
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Category item);
        Task<bool> UpdateAsync(CategoryRequest request);
        Task<bool> DeleteByIdAsync(int id);
    }
}
