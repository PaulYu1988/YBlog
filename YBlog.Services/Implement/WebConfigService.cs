using Microsoft.EntityFrameworkCore;
using System.Reflection;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;

namespace YBlog.Services.Implement
{
    public class WebConfigService : IWebConfigService
    {
        protected YBlogContext _context;
        public WebConfigService(YBlogContext context) { 
            _context = context;
        }
        public async Task<List<WebConfig>> GetAllAsync()
        {
            return await _context.WebConfigs.ToListAsync();
        }

        public async Task<bool> UpdateAsync(WebConfigView item, Dictionary<string, string> files)
        {
            var items = await GetAllAsync();
            var t = item.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                string name = pi.Name;
                var curr = items.FirstOrDefault(x => x.ConfigKey == name);
                if (curr != null)
                {
                    var fileName = name + "File";
                    if (files.Any(x => x.Key == fileName))
                    {
                        curr.ConfigValue = files[fileName];
                        curr.LastModifiedAt = DateTime.Now;
                    }
                    else
                    {
                        var obj = pi.GetValue(item);
                        var value = obj?.ToString();
                        curr.ConfigValue = value;
                        curr.LastModifiedAt = DateTime.Now;
                    }
                }
            }
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
