using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;

namespace YBlog.Services.Mock
{
    public class MockCacheService : ICacheService
    {
        protected IMemoryCache _memoryCache;
        public MockCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void Clear(EnumCacheNames cacheName)
        {

        }

        public List<Article> GetTopArticles(int take = 8)
        {
            var items = new List<Article>();
            for (var i = 0; i < take; i++)
            {
                items.Add(new Article()
                {
                    Title = "Recommand article title" + i,
                    Id = i + 1
                });
            }
            return items;
        }

        public List<Tag> GetTopTags(int take = 32)
        {
            var items = new List<Tag>();
            for (var i = 0; i < take; i++)
            {
                items.Add(new Tag()
                {
                    Text = "Tags" + i,
                    Id = i + 1
                });
            }
            return items;
        }

        public List<Nav> GetNavs()
        {
            var items = new List<Nav>();
            items.Add(new Nav()
            {
                Id = 1,
                Text = "Index",
                Url = "/",
                Target = 1,
            });
            items.Add(new Nav()
            {
                Id = 2,
                Text = ".NET",
                Url = "",
                Target = 1
            });
            items.Add(new Nav()
            {
                Id = 3,
                Text = "Angular",
                Url = "/categories/2",
                Target = 1,
            });
            items.Add(new Nav()
            {
                Id = 4,
                Text = "React",
                Url = "/categories/3",
                Target = 1,
            });
            items.Add(new Nav()
            {
                Id = 5,
                Text = "Next.js",
                Url = "/categories/4",
                Target = 1,
                ParentId = 2
            });
            items.Add(new Nav()
            {
                Id = 6,
                Text = "Share",
                Url = "/categories/5",
                Target = 1,
                ParentId = 2
            });
            return items;
        }

        public WebConfigView GetWebConfigView()
        {
            return new WebConfigView()
            {
                SiteName = "YBlog",
                Logo = "/favicon.ico",
                Copyright = @"<div class=""layui-text""><div style=""text-align: center;"">Copyright &copy; 2024 <a href="""">YBlog</a> <a href=""https://beian.miit.gov.cn/"" target=""_blank"">沪ICP备10219633号-8</a> <a href="""">网站地图</a><br />Power by <a href="""">Leavescn</a></div></div>"
            };
        }

        public List<Link> GetLinks()
        {
            return GetList();
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
            return GetAdvertisementList();
        }
        private List<Advertisement> GetAdvertisementList()
        {
            var items = new List<Advertisement>();
            items.Add(new Advertisement
            {
                Code = "HtmlHead",
                AdvertisementContent = "<script>console.log('HtmlHead');</script>",
                IsEnabled = true,
                CreatedAt = DateTime.Now,
                Id = 1,
                Name = "Hmtl头部",
                LastModifiedAt = DateTime.Now,
            });
            items.Add(new Advertisement
            {
                Code = "NavBottom",
                AdvertisementContent = "<script>console.log('NavBottom');</script>",
                IsEnabled = false,
                CreatedAt = DateTime.Now,
                Id = 2,
                Name = "导航下方",
                LastModifiedAt = DateTime.Now,
            });
            return items;
        }

        public string GetAssetsUrl(string path)
        {
            return path;
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