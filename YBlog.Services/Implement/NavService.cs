using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Requests;

namespace YBlog.Services.Implement
{
    public class NavService : INavService
    {
        protected YBlogContext _context;
        public NavService(YBlogContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Nav item)
        {
            _context.Navs.Add(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Task<Nav?> GetByIdAsync(int id)
        {
            return _context.Navs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Nav>> GetAllAsync() {
            return _context.Navs.OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<bool> UpdateAsync(NavRequest request)
        {
            if (!request.Id.HasValue)
                return false;
            var curr = await GetByIdAsync(request.Id.Value);
            if (curr == null)
                return false;
            curr.Text = request.Text ?? string.Empty;
            curr.Sort = request.Sort;
            curr.Target = request.Target ?? ((int)EnumTargets._blank);
            curr.Url = request.Url ?? string.Empty;
            curr.IsEnabled = request.IsEnabled;
            curr.LastModifiedAt = DateTime.Now;
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await _context.Navs.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }
            _context.Navs.Remove(item);
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
