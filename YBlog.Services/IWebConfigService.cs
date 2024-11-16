using YBlog.Models.Custom;
using YBlog.Models.Database;

namespace YBlog.Services
{
    public interface IWebConfigService
    {
        Task<List<WebConfig>> GetAllAsync();
        Task<bool> UpdateAsync(WebConfigView item, Dictionary<string, string> files);
    }
}
