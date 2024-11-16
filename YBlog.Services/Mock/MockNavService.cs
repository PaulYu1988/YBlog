using YBlog.Models.Database;
using YBlog.Models.Requests;
using YBlog.Models.Enums;

namespace YBlog.Services.Mock
{
    public class MockNavService : INavService
    {
        public Task<bool> CreateAsync(Nav item)
        {
            return Task.FromResult(true);
        }

        public Task<Nav?> GetByIdAsync(int id)
        {
            Nav? nav = null;
            if (id > 0)
            {
                nav = GetMockList(10).FirstOrDefault(x => x.Id == id);
            }
            return Task.FromResult(nav);
        }

        public Task<bool> UpdateAsync(NavRequest request)
        {
            return Task.FromResult(true);
        }

        private List<Nav> GetMockList(int take)
        {
            var items = new List<Nav>();
            for (var i = 0; i < take; i++)
            {
                var item = new Nav()
                {
                    Id = i + 1,
                    ParentId = i % 2 == 0 ? 2 : null,
                    CreatedAt = DateTime.Now.AddDays(0 - i),
                    LastModifiedAt = DateTime.Now.AddDays(0 - i),
                    IsEnabled = true,
                    Sort = 50 + i,
                    Target = (int)EnumTargets._blank,
                    Text = "Text" + i,
                    Url = "https://google.com"
                };
                items.Add(item);
            }
            return items.OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToList();
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(id % 2 == 0 ? true : false);
        }

        public Task<List<Nav>> GetAllAsync()
        {
            return Task.FromResult(GetMockList(10));
        }
    }
}
