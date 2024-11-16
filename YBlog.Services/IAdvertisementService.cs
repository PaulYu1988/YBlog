using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface IAdvertisementService
    {
        Task<List<Advertisement>> GetAsync(AdvertisementPagedQuery query);
        Task<Advertisement?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(AdvertisementRequest request);
    }
}
