using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface ILinkService
    {
        Task<List<Link>> GetAsync(LinkPagedQuery query);
        Task<Link?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(LinkRequest request);
        Task<bool> CreateAsync(Link item);
        Task<bool> DeleteByIdAsync(int id);
    }
}
