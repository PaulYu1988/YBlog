using YBlog.Models.Database;
using YBlog.Models.Requests;

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
            items.Add(new Nav()
            {
                Id = 1,
                Text = "Index",
                Url = "/",
                Target = 1,
                IsEnabled = true
            });
            items.Add(new Nav()
            {
                Id = 2,
                Text = ".NET",
                Url = "",
                Target = 1,
                IsEnabled = true
            });
            items.Add(new Nav()
            {
                Id = 3,
                Text = "Angular",
                Url = "/categories/2",
                Target = 1,
                IsEnabled = true
            });
            items.Add(new Nav()
            {
                Id = 4,
                Text = "React",
                Url = "/categories/3",
                Target = 1,
                IsEnabled = true
            });
            items.Add(new Nav()
            {
                Id = 5,
                Text = "Next.js",
                Url = "/categories/4",
                Target = 1,
                ParentId = 2,
                IsEnabled = true
            });
            items.Add(new Nav()
            {
                Id = 6,
                Text = "Share",
                Url = "/categories/5",
                Target = 1,
                ParentId = 2
            });
            items.Add(new Nav()
            {
                Id = 7,
                Text = "Angular.js",
                Url = "/categories/5",
                Target = 1,
                ParentId = 3,
                IsEnabled = true
            });
            return items;
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
