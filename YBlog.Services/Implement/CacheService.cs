using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text.RegularExpressions;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;

namespace YBlog.Services.Implement
{
    public class CacheService : ICacheService
    {
        protected IMemoryCache _memoryCache;
        protected YBlogContext _context;
        public CacheService(IMemoryCache memoryCache, YBlogContext context)
        {
            _memoryCache = memoryCache;
            _context = context;
        }
        public void Clear(EnumCacheNames cacheName)
        {
            _memoryCache.Remove(cacheName.ToString());
        }

        public List<Article> GetTopArticles(int take = 8)
        {
            var cacheKey = EnumCacheNames.TopArticles.ToString();
            var cacheItems = _memoryCache.Get<List<Article>>(cacheKey);
            if (cacheItems != null)
            {
                return cacheItems;
            }
            var items = _context.Articles.Where(x => x.IsDeleted == false).OrderByDescending(x => x.ReadCount).Take(take).ToList();
            if (items.Any() && items.Count >= take)
            {
                _memoryCache.Set(cacheKey, items, TimeSpan.FromHours(4));
            }
            return items;
        }

        public List<Tag> GetTopTags(int take = 32)
        {
            var cacheKey = EnumCacheNames.HotTags.ToString();
            var cacheItems = _memoryCache.Get<List<Tag>>(cacheKey);
            if (cacheItems != null)
            {
                return cacheItems;
            }
            var items = (from t in _context.Tags
                         join at in _context.ArticleTags
                         on t.Id equals at.TagId
                         group t by new { t.Id, t.Text } into g
                         orderby g.Count() descending
                         select new Tag() { Id = g.Key.Id, Text = g.Key.Text }).Take(take).ToList();
            if (items.Any() && items.Count >= take)
            {
                _memoryCache.Set(cacheKey, items, TimeSpan.FromHours(4));
            }
            return items;
        }

        public List<Nav> GetNavs()
        {
            var cacheKey = EnumCacheNames.Navs.ToString();
            var cacheItems = _memoryCache.Get<List<Nav>>(cacheKey);
            if (cacheItems != null)
            {
                return cacheItems;
            }
            var navs = _context.Navs.Where(x => x.IsEnabled == true).OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToList();
            _memoryCache.Set(cacheKey, navs, TimeSpan.FromDays(30));
            return navs;
        }

        public WebConfigView GetWebConfigView()
        {
            var cacheKey = EnumCacheNames.WebConfigs.ToString();
            var cacheWebConfigs = _memoryCache.Get<WebConfigView>(cacheKey);
            if (cacheWebConfigs != null)
            {
                return cacheWebConfigs;
            }
            else
            {
                var webConfig = new WebConfigView();
                var webConfigList = _context.WebConfigs.ToList();
                if (webConfigList.Any())
                {
                    Type t = webConfig.GetType();
                    foreach (PropertyInfo pi in t.GetProperties())
                    {
                        string name = pi.Name;
                        var curr = webConfigList.FirstOrDefault(x => x.ConfigKey == name);
                        if (curr != null)
                        {
                            pi.SetValue(webConfig, curr.ConfigValue);
                        }
                    }
                    _memoryCache.Set(cacheKey, webConfig);
                }
                return webConfig;
            }
        }

        public List<Link> GetLinks()
        {
            var cacheKey = EnumCacheNames.Links.ToString();
            var cacheItems = _memoryCache.Get<List<Link>>(cacheKey);
            if (cacheItems != null)
            {
                return cacheItems;
            }
            var links = _context.Links.Where(x => x.IsEnabled == true).OrderByDescending(x => x.Sort).ThenByDescending(x => x.Id).ToList();
            _memoryCache.Set(cacheKey, links, TimeSpan.FromDays(30));
            return links;
        }

        public List<ManageMenu> GetManageMenus(string path)
        {
            var items = GetDefaultMenus();
            foreach (var menu in items)
            {
                foreach (var item in menu.Items)
                {
                    if (path == "/manage" && path == item.Url)
                    {
                        item.Active = true;
                        menu.Active = true;
                    }
                    else if (item.Url != "/manage" && path.StartsWith(item.Url ?? string.Empty))
                    {
                        item.Active = true;
                        menu.Active = true;
                    }
                }
            }
            return items;
        }

        private List<ManageMenu> GetDefaultMenus()
        {
            var items = new List<ManageMenu>();
            var siteManage = new ManageMenu()
            {
                Text = "站点管理",
                Icon = "layui-icon-util"
            };
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "控制台",
                Icon = "layui-icon-console",
                Url = "/manage"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "导航",
                Icon = "layui-icon-transfer",
                Url = "/manage/navs"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "用户",
                Icon = "layui-icon-username",
                Url = "/manage/users"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "设置",
                Icon = "layui-icon-set",
                Url = "/manage/webconfigs"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "Sitemap",
                Icon = "layui-icon-website",
                Url = "/manage/sitemaps"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "资源文件",
                Icon = "layui-icon-file",
                Url = "/manage/files"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "操作记录",
                Icon = "layui-icon-read",
                Url = "/manage/operations"
            });
            siteManage.Items.Add(new ManageMenuItem()
            {
                Text = "日志",
                Icon = "layui-icon-survey",
                Url = "/manage/logs"
            });
            items.Add(siteManage);
            var contentManage = new ManageMenu()
            {
                Text = "内容管理",
                Icon = "layui-icon-app"
            };
            contentManage.Items.Add(new ManageMenuItem()
            {
                Text = "分类",
                Icon = "layui-icon-template-1",
                Url = "/manage/categories"
            });
            contentManage.Items.Add(new ManageMenuItem()
            {
                Text = "文章",
                Icon = "layui-icon-list",
                Url = "/manage/articles"
            });
            contentManage.Items.Add(new ManageMenuItem()
            {
                Text = "标签",
                Icon = "layui-icon-note",
                Url = "/manage/tags"
            });
            contentManage.Items.Add(new ManageMenuItem()
            {
                Text = "评论",
                Icon = "layui-icon-dialogue",
                Url = "/manage/comments"
            });
            items.Add(contentManage);
            var marketManage = new ManageMenu()
            {
                Text = "运营管理",
                Icon = "layui-icon-test"
            };
            marketManage.Items.Add(new ManageMenuItem()
            {
                Text = "友情链接",
                Icon = "layui-icon-link",
                Url = "/manage/links"
            });
            marketManage.Items.Add(new ManageMenuItem()
            {
                Text = "广告代码",
                Icon = "layui-icon-fonts-code",
                Url = "/manage/advertisements"
            });
            items.Add(marketManage);
            return items;
        }

        public List<Advertisement> GetAdvertisements()
        {
            var cacheKey = EnumCacheNames.Advertisements.ToString();
            var cacheAdvertisements = _memoryCache.Get<List<Advertisement>>(cacheKey);
            if (cacheAdvertisements != null)
            {
                return cacheAdvertisements;
            }
            var advertisementList = _context.Advertisements.Where(x => x.IsEnabled == true).ToList();
            _memoryCache.Set(cacheKey, advertisementList, TimeSpan.FromDays(30));
            return advertisementList;
        }

        public string GetAssetsUrl(string path)
        {
            var webConfigs = GetWebConfigView();
            if (string.IsNullOrWhiteSpace(webConfigs.AssetsCDN))
            {
                return path;
            }
            return $"{webConfigs.AssetsCDN}{path}";
        }

        public string GetAbbreviation(string? nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                return "匿";
            }
            if (nickname.Length < 2)
            {
                return nickname;
            }
            var first = nickname.Substring(0, 1);
            if (Regex.IsMatch(first, @"[\u4e00-\u9fa5]"))
            {
                return first;
            }
            var second = nickname.Substring(1, 1);
            if (Regex.IsMatch(second, @"[\u4e00-\u9fa5]"))
            {
                return second;
            }
            return (first + second).ToUpper();
        }

        public string GetAvatarBackgroundColor(int? userId)
        {
            if (!userId.HasValue)
            {
                return "#16baaa";
            }
            var cacheKey = "Nickname" + userId;
            var curr = _memoryCache.Get<string>(cacheKey);
            if (!string.IsNullOrWhiteSpace(curr))
            {
                return curr;
            }
            string[] colors = {
                    "#ADD8E6", "#87CEEB", "#1E90FF", "#4169E1",  // 蓝色系
                    "#98FF98", "#90EE90", "#32CD32", "#228B22",  // 绿色系
                    "#FFD700", "#FFEE58", "#FFC107",  // 黄色系
                    "#E6E6FA", "#D8BFD8", "#EE82EE", "#8A2BE2",  // 紫色系
                    "#FFDAB9", "#FFA500", "#FF8C00", "#FF4500",  // 橙色系
                    "#FFB6C1", "#FF69B4", "#FF6347", "#DC143C"   // 红色系
                };
            var len = colors.Length;
            var rnd = new Random(userId.Value + DateTime.Now.Second);
            var num = rnd.Next(0, len);
            var color = colors[num];
            _memoryCache.Set(cacheKey, color);
            return color;
        }
    }
}