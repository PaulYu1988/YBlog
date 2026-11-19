using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockBotService : IBotService
    {
        private List<Bot> GetList()
        {
            return new List<Bot>
            {
                new Bot
                {
                    Id = 1,
                    Name = "Googlebot",
                    IdentificationCode = "Googlebot",
                    Type = 1,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now
                },
                new Bot
                {
                    Id = 2,
                    Name = "GPTBot",
                    IdentificationCode = "GPTBot",
                    Type = 2,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now
                }
            };
        }

        public Task<List<Bot>> GetAsync(BotPagedQuery query)
        {
            var items = GetList();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                items = items.Where(x => x.Name.Contains(query.Name)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(query.IdentificationCode))
            {
                items = items.Where(x => x.IdentificationCode.Contains(query.IdentificationCode)).ToList();
            }
            if (query.Type.HasValue)
            {
                items = items.Where(x => x.Type == query.Type.Value).ToList();
            }
            query.TotalCount = items.Count;
            query.PageCount = items.Count == 0 ? 0 : (items.Count + query.PageSize - 1) / query.PageSize;
            return Task.FromResult(items.Skip(((query.Page ?? 1) - 1) * query.PageSize).Take(query.PageSize).ToList());
        }

        public Task<Bot?> GetByIdAsync(int id)
        {
            return Task.FromResult(GetList().FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> CreateAsync(Bot item)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(BotRequest request)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(GetList().Any(x => x.Id == id));
        }
    }
}
