using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class AdvertisementService : IAdvertisementService
    {
        protected YBlogContext _context;
        public AdvertisementService(YBlogContext context)
        {
            _context = context;
        }

        public Task<List<Advertisement>> GetAsync(AdvertisementPagedQuery query)
        {
            var queryable = _context.Advertisements.AsQueryable();
            if (query.IsEnabled.HasValue)
            {
                queryable = queryable.Where(x => x.IsEnabled == query.IsEnabled);
            }
            var items = queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public Task<Advertisement?> GetByIdAsync(int id)
        {
            return _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(AdvertisementRequest request)
        {
            var item = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (item != null)
            {
                item.AdvertisementContent = request.AdvertisementContent;
                item.IsEnabled = request.IsEnabled;
                item.LastModifiedAt = DateTime.Now;
            }
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
