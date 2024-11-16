using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class LinkService : ILinkService
    {
        protected YBlogContext _context;
        public LinkService(YBlogContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Link item)
        {
            _context.Links.Add(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await _context.Links.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }
            _context.Links.Remove(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Task<List<Link>> GetAsync(LinkPagedQuery query)
        {
            var queryable = _context.Links.AsQueryable();
            if (query.IsEnabled.HasValue)
            {
                queryable = queryable.Where(x => x.IsEnabled == query.IsEnabled);
            }
            if (!string.IsNullOrEmpty(query.Text))
            {
                queryable = queryable.Where(x => x.Text.Contains(query.Text));
            }
            var items = queryable.OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public Task<Link?> GetByIdAsync(int id)
        {
            return _context.Links.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(LinkRequest request)
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
    }
}
