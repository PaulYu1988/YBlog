using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        protected YBlogContext _context;
        public CategoryService(YBlogContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Category item)
        {
            await _context.Categories.AddRangeAsync(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }
            item.IsDeleted = true;
            item.LastModifiedAt = DateTime.Now;
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.Where(x => !x.IsDeleted).OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<List<Category>> GetAsync(CategoryPagedQuery query)
        {
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryable = queryable.Where(x => x.Name.Contains(query.Name));
            }
            var items = await queryable.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }

        public async Task<bool> UpdateAsync(CategoryRequest request)
        {
            var result = false;
            if (request.Id.HasValue)
            {
                var curr = await GetByIdAsync(request.Id.Value);
                if (curr != null)
                {
                    curr.Name = request.Name ?? string.Empty;
                    curr.Sort = request.Sort;
                    curr.Description = request.Description;
                    curr.LastModifiedAt = DateTime.Now;
                    result = (await _context.SaveChangesAsync()) > 0;
                }
            }
            return result;
        }
    }
}