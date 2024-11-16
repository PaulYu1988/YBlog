using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockCategoryService : ICategoryService
    {
        public Task<bool> CreateAsync(Category item)
        {
            item.Id = 1;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return Task.FromResult(id % 2 == 0 ? true : false);
        }

        public Task<List<Category>> GetAllAsync()
        {
            var items = GetMockList();
            items = items.Take(5).ToList();
            return Task.FromResult(items);
        }

        public Task<List<Category>> GetAsync(CategoryPagedQuery query)
        {
            var items = GetMockList();
            items = items.Take(query.PageSize).ToList();
            return Task.FromResult(items);
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            var item = GetMockList().FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<bool> UpdateAsync(CategoryRequest request)
        {
            return Task.FromResult(request.Id % 2 == 0 ? true : false);
        }

        private List<Category> GetMockList()
        {
            var items = new List<Category>();
            for (var i = 0; i < 20; i++)
            {
                var item = new Category()
                {
                    Id = i + 1,
                    Name = "分类" + i,
                    Description = "分类" + i + "描述",
                    IsDeleted = false,
                    CreatedAt = DateTime.Now.AddDays(0 - i),
                    LastModifiedAt = DateTime.Now.AddHours(0 - 1),
                    Sort = i
                };
                items.Add(item);
            }
            return items;
        }
    }
}
