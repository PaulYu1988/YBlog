using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;

namespace YBlog.Services.Mock
{
    public class MockLogService : ILogService
    {
        public Task<List<Log>> GetAsync(LogPagedQuery query)
        {
            return Task.FromResult(GetList(query.PageSize));
        }

        public void Log(EnumLogTypes type, string? message)
        {

        }

        private List<Log> GetList(int pageSize)
        {
            var items = new List<Log>();
            for (var i = 0; i < pageSize; i++)
            {
                items.Add(new Log()
                {
                    Id = i + 1,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now,
                    LogContent = "要在 CSS 中实现文本超出显示省略号（ellipsis），您可以使用 text-overflow: ellipsis 属性。此外，您还需要设置 overflow: hidden 和 white-space: nowrap 以确保文本不换行并且超出时隐藏。",
                    Type = new Random().Next(1, 4)
                });
            }
            return items;
        }
    }
}
