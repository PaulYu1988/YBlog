using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class LogService : ILogService
    {
        protected YBlogContext _context;
        public LogService(YBlogContext context)
        {
            _context = context;
        }

        public Task<List<Log>> GetAsync(LogPagedQuery query)
        {
            var queryable = _context.Logs.AsQueryable();
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(x => x.Type == query.Type);
            }
            return queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
        }

        public void Log(EnumLogTypes type, string? message)
        {
            try
            {
                var log = new Log()
                {
                    Type = (int)type,
                    LogContent = message
                };
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch
            {

            }
        }
    }
}
