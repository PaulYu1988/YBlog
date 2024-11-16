using YBlog.Models.Custom;
using YBlog.Models.Database;

namespace YBlog.Services.Mock
{
    public class MockWebConfigService : IWebConfigService
    {
        public Task<List<WebConfig>> GetAllAsync()
        {
            var items = new List<WebConfig>();
            items.Add(new WebConfig()
            {
                Id = 1,
                Name = "网站名称",
                ConfigKey = "SiteName",
                ConfigValue = "SiteName",
                Description = "SiteName"
            });
            items.Add(new WebConfig()
            {
                Id = 2,
                Name = "Logo",
                ConfigKey = "Logo",
                ConfigValue = "https://www.leavescn.com/favicon.ico"
            });
            items.Add(new WebConfig()
            {
                Id = 3,
                Name = "SiteAddress",
                ConfigKey = "SiteAddress"
            });
            items.Add(new WebConfig()
            {
                Id = 4,
                Name = "Copyright",
                ConfigKey = "Copyright"
            });
            items.Add(new WebConfig()
            {
                Id = 5,
                Name = "邮箱",
                ConfigKey = "Email"
            });
            items.Add(new WebConfig()
            {
                Id = 6,
                Name = "水印",
                ConfigKey = "Watermark"
            });
            items.Add(new WebConfig()
            {
                Id = 7,
                Name = "水印图片最小宽度",
                ConfigKey = "WatermarkStartWidth"
            });
            items.Add(new WebConfig()
            {
                Id = 8,
                Name = "Analytics",
                ConfigKey = "Analytics"
            });
            items.Add(new WebConfig()
            {
                Id = 9,
                Name = "MetaKeywords",
                ConfigKey = "MetaKeywords"
            });
            items.Add(new WebConfig()
            {
                Id = 10,
                Name = "MetaDescription",
                ConfigKey = "MetaDescription"
            });
            items.Add(new WebConfig()
            {
                Id = 11,
                Name = "AssetsCDN",
                ConfigKey = "AssetsCDN"
            });
            return Task.FromResult(items);
        }

        public Task<bool> UpdateAsync(WebConfigView item, Dictionary<string, string> files)
        {
            return Task.FromResult(true);
        }
    }
}
