using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockLinkService : ILinkService
    {
        public Task<bool> CreateAsync(Link item)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(id % 2 == 0);
        }

        public Task<List<Link>> GetAsync(LinkPagedQuery query)
        {
            var items = GetList();
            if (query.IsEnabled.HasValue)
            {
                items = items.Where(x => x.IsEnabled == query.IsEnabled).ToList();
            }
            if (!string.IsNullOrEmpty(query.Text))
            {
                items = items.Where(x => x.Text.Contains(query.Text)).ToList();
            }
            return Task.FromResult(items);
        }

        public Task<Link?> GetByIdAsync(int id)
        {
            var items = GetList();
            return Task.FromResult(items.FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> UpdateAsync(LinkRequest request)
        {
            return Task.FromResult(request.Id % 2 == 0);
        }

        private List<Link> GetList()
        {
            var items = new List<Link>();
            items.Add(new Link()
            {
                Id = 1,
                Text = "Leavescn",
                Url = "https://www.leavescn.com",
                IsEnabled = true,
                Sort = 50,
                Target = (int)EnumTargets._blank,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now
            });
            items.Add(new Link()
            {
                Id = 2,
                Text = ".NET绿叶社区",
                Url = "https://www.leavescn.com",
                IsEnabled = false,
                Sort = 50,
                Target = (int)EnumTargets._blank,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now
            });
            return items;
        }
    }
}
