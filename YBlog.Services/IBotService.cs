using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface IBotService
    {
        Task<List<Bot>> GetAsync(BotPagedQuery query);
        Task<Bot?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Bot item);
        Task<bool> UpdateAsync(BotRequest request);
        Task<bool> DeleteByIdAsync(int id);
    }
}
