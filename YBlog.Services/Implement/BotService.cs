using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class BotService : IBotService
    {
        protected YBlogContext _context;

        public BotService(YBlogContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(Bot item)
        {
            _context.Bots.Add(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item == null)
            {
                return false;
            }
            _context.Bots.Remove(item);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Task<List<Bot>> GetAsync(BotPagedQuery query)
        {
            var queryable = _context.Bots.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryable = queryable.Where(x => x.Name.Contains(query.Name));
            }
            if (!string.IsNullOrWhiteSpace(query.IdentificationCode))
            {
                queryable = queryable.Where(x => x.IdentificationCode.Contains(query.IdentificationCode));
            }
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(x => x.Type == query.Type.Value);
            }
            return queryable.OrderBy(x => x.Id).ToPagedListAsync(query);
        }

        public Task<Bot?> GetByIdAsync(int id)
        {
            return _context.Bots.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(BotRequest request)
        {
            if (!request.Id.HasValue)
            {
                return false;
            }
            var item = await GetByIdAsync(request.Id.Value);
            if (item == null)
            {
                return false;
            }
            item.Name = request.Name!;
            item.IdentificationCode = request.IdentificationCode!;
            item.Type = request.Type!.Value;
            item.LastModifiedAt = DateTime.Now;
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
