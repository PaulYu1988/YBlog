using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;

namespace YBlog.Services
{
    public interface ILogService
    {
        void Log(EnumLogTypes type, string? message);
        Task<List<Log>> GetAsync(LogPagedQuery query);
    }
}
