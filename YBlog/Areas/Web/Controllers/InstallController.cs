using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YBlog.Extensions;
using YBlog.Models.Database;
using YBlog.Models.Enums;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class InstallController : Controller
    {
        protected YBlogContext _context;
        protected IConfiguration _configuration;
        protected IWebHostEnvironment _env;
        public InstallController(IConfiguration configuration, YBlogContext context, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _context = context;
            _env = env;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var installed = _configuration.GetValue<bool?>("Installed");
            if (installed == true)
            {
                return Redirect("/");
            }
            var connectionString = _configuration.GetConnectionString("YBlog");
            ViewBag.ConnectionString = connectionString;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                ViewBag.CanConnect = _context.Database.CanConnect();
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(InstallRequest request)
        {
            try
            {
                var filePath = _env.ContentRootPath + "\\appsettings.json";
                var currAppSettings = System.IO.File.ReadAllText(filePath);
                try
                {
                    using (var sw = new StreamWriter(filePath))
                    {
                        sw.Write(currAppSettings);
                    }
                }
                catch (Exception ex)
                {
                    return this.InternalServerError(new
                    {
                        message = "写入appsettings.json配置文件失败，请检查该文件是否有写入权限。" + ex.Message
                    });
                }
                _context.Database.ExecuteSqlRaw(InstallSettings.SQLCreateTables);
                var user = new User()
                {
                    Email = request.Email,
                    Nickname = request.Nickname ?? string.Empty,
                    Type = (int)EnumUserTypes.Admin,
                    Status = (int)EnumUserStatuses.Enabled,
                    RegIp = Request.GetRequestIpAddress()
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                var userCredential = new UserCredential()
                {
                    UserId = user.Id,
                    Password = request.Password.ToMD5(),
                    Username = request.Username ?? string.Empty
                };
                _context.UserCredentials.Add(userCredential);
                _context.SaveChanges();
                var category = new Category()
                {
                    Name = "示例分类",
                    Sort = 50,
                    Description = "这是一个示例分类，描述内容会输出在meta description标签里"
                };
                _context.Categories.Add(category);
                _context.SaveChanges();
                var article1 = new Article()
                {
                    Title = "示例文章 使用YBlog添加的第一篇文章",
                    Description = "这是一篇示例文章，是使用YBlog添加的第一篇文章。文章内容仅供测试使用，您可以在后台删除或者编辑这篇文章。",
                    ArticleContent = "<h2>.NET8</h2><p>ASP.NET 核心运行时使你能够运行现有的 Web/服务器应用程序。在 Windows 上，我们建议安装托管捆绑包，其中包括 .NET 运行时和 IIS 支持。</p><h2>数据库</h2><p>建议SQL Server 2016及以上版本。</p><p>低于2016版本请将appsettings.json中的CompatibilityLevel配置为120,否则EF Core 8会有兼容性问题导致报错。</p>",
                    CategoryId = category.Id,
                    Status = (int)EnumArticleStatuses.Publish,
                    UserId = user.Id
                };
                var article2 = new Article()
                {
                    Title = "示例文章 使用YBlog添加的第二篇文章",
                    Description = "这是一篇示例文章，是使用YBlog添加的第一篇文章。文章内容仅供测试使用，您可以在后台删除或者编辑这篇文章。",
                    ArticleContent = "<h2>.NET8</h2><p>ASP.NET 核心运行时使你能够运行现有的 Web/服务器应用程序。在 Windows 上，我们建议安装托管捆绑包，其中包括 .NET 运行时和 IIS 支持。</p><h2>数据库</h2><p>建议SQL Server 2016及以上版本。</p><p>低于2016版本请将appsettings.json中的CompatibilityLevel配置为120,否则EF Core 8会有兼容性问题导致报错。</p>",
                    CategoryId = category.Id,
                    Status = (int)EnumArticleStatuses.Publish,
                    UserId = user.Id
                };
                _context.Articles.Add(article1);
                _context.Articles.Add(article2);
                _context.SaveChanges();
                var tag = new Tag()
                {
                    Text = "示例标签"
                };
                _context.Tags.Add(tag);
                _context.SaveChanges();
                var articleTag = new ArticleTag()
                {
                    ArticleId = article1.Id,
                    TagId = tag.Id,
                    Sort = 1
                };
                _context.ArticleTags.Add(articleTag);
                _context.SaveChanges();
                var nav = new Nav()
                {
                    Text = "示例分类",
                    Sort = 50,
                    Target = (int)EnumTargets._self,
                    Url = "/categories/" + category.Id,
                    IsEnabled = true
                };
                _context.Navs.Add(nav);
                _context.SaveChanges();
                var link1 = new Link()
                {
                    Sort = 50,
                    Text = "YBlog",
                    Url = "https://www.y617.com",
                    Target = (int)EnumTargets._blank,
                    IsEnabled = true
                };
                var link2 = new Link()
                {
                    Sort = 50,
                    Text = ".NET绿叶社区",
                    Url = "https://www.leavescn.com",
                    Target = (int)EnumTargets._blank,
                    IsEnabled = true
                };
                _context.Links.Add(link1);
                _context.Links.Add(link2);
                _context.SaveChanges();
                var siteName = new WebConfig()
                {
                    Name = "站点名称",
                    ConfigKey = "SiteName",
                    ConfigValue = request.SiteName
                };
                _context.WebConfigs.Add(siteName);
                var logo = new WebConfig()
                {
                    Name = "网站Logo",
                    ConfigKey = "Logo",
                    ConfigValue = "/favicon.ico"
                };
                _context.WebConfigs.Add(logo);
                var siteAddress = new WebConfig()
                {
                    Name = "站点地址",
                    ConfigKey = "SiteAddress",
                    ConfigValue = request.SiteAddress
                };
                _context.WebConfigs.Add(siteAddress);
                var copyright = new WebConfig()
                {
                    Name = "版权信息",
                    ConfigKey = "Copyright",
                    ConfigValue = "<div class=\"layui-text\"><div style=\"text-align: center;\">Copyright © 2024 <a href=\"/\">" + request.SiteName + "</a> <a href=\"/sitemap.xml\">网站地图</a><br>Power by <a href=\"https://www.y617.com\">YBlog</a></div></div>"
                };
                _context.WebConfigs.Add(copyright);
                var email = new WebConfig()
                {
                    Name = "站长邮箱",
                    ConfigKey = "Email",
                    ConfigValue = request.Email
                };
                _context.WebConfigs.Add(email);
                var analytics = new WebConfig()
                {
                    Name = "统计代码",
                    ConfigKey = "Analytics"
                };
                _context.WebConfigs.Add(analytics);
                var metaKeywords = new WebConfig()
                {
                    Name = "网站关键字（SEO）",
                    ConfigKey = "MetaKeywords"
                };
                _context.WebConfigs.Add(metaKeywords);
                var metaDescription = new WebConfig()
                {
                    Name = "网站描述（SEO）",
                    ConfigKey = "MetaDescription"
                };
                _context.WebConfigs.Add(metaDescription);
                var assetsCDN = new WebConfig()
                {
                    Name = "资源文件夹CDN地址",
                    ConfigKey = "AssetsCDN",
                    Description = "为Assets资源文件夹设置CDN地址，将会从CDN加载Assets资源文件夹里的文件，例如：https://cdn.domain.com"
                };
                _context.WebConfigs.Add(assetsCDN);
                var adIPWhitelist = new WebConfig()
                {
                    Name = "广告IP白名单",
                    ConfigKey = "AdIPWhitelist",
                    Description = "对于配置的IP地址不显示广告，多个IP请以英文逗号(,)隔开"
                };
                _context.WebConfigs.Add(adIPWhitelist);
                var keywordBlacklist = new WebConfig()
                {
                    Name = "敏感词黑名单",
                    ConfigKey = "KeywordBlacklist",
                    Description = "禁止评论包含关键词，多个关键词请以英文逗号(,)隔开。"
                };
                _context.WebConfigs.Add(keywordBlacklist);
                var dailyCommentLimit = new WebConfig()
                {
                    Name = "每日评论次数限制",
                    ConfigKey = "DailyCommentLimit",
                    Description = "用户每日评论次数限制，防止恶意评论。"
                };
                _context.WebConfigs.Add(dailyCommentLimit);
                _context.SaveChanges();
                var connectionString = _configuration.GetConnectionString("YBlog");
                var compatibilityLevel = _configuration.GetValue<int?>("CompatibilityLevel");
                var newAppSettings = compatibilityLevel.HasValue ? InstallSettings.AppSettingsWithCompatibilityLevel : InstallSettings.AppSettings;
                if (compatibilityLevel.HasValue)
                {
                    newAppSettings = newAppSettings.Replace("{CompatibilityLevel}", compatibilityLevel.Value.ToString());
                }
                newAppSettings = newAppSettings.Replace("{ConnectionStrings}", connectionString);
                using (var sw = new StreamWriter(filePath))
                {
                    sw.Write(newAppSettings);
                }
                return this.Success();
            }
            catch (Exception ex)
            {
                return this.InternalServerError(new
                {
                    ex.Message
                });
            }
        }
    }
    public class InstallRequest
    {
        public string? SiteName { get; set; }
        public string? SiteAddress { get; set; }
        public string? Username { get; set; }
        public string? Nickname { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }

    public class InstallSettings
    {
        public static string AppSettings = @"
{
  ""ConnectionStrings"": {
    ""YBlog"": ""{ConnectionStrings}""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }
  },
  ""AllowedHosts"": ""*"",
  ""Installed"": true
}
";
        public static string AppSettingsWithCompatibilityLevel = @"
{
  ""ConnectionStrings"": {
    ""YBlog"": ""{ConnectionStrings}""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }
  },
  ""AllowedHosts"": ""*"",
  ""CompatibilityLevel"": {CompatibilityLevel},
  ""Installed"": true
}
";
        public static string SQLCreateTables = @"CREATE TABLE [dbo].[Advertisements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,
	[AdvertisementContent] [nvarchar](max) NULL,
	[IsEnabled] [bit] NOT NULL CONSTRAINT [DF_Advertisements_IsEnabled]  DEFAULT ((1)),
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Advertisements_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Advertisements_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_Advertisements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Articles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[ArticleContent] [nvarchar](max) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[IsTop] [bit] NOT NULL CONSTRAINT [DF_Articles_IsTop]  DEFAULT ((0)),
	[Thumbnail] [nvarchar](max) NULL,
	[ReadCount] [int] NOT NULL CONSTRAINT [DF_Articles_ReadCount]  DEFAULT ((0)),
	[CommentCount] [int] NOT NULL CONSTRAINT [DF_Articles_CommentCount]  DEFAULT ((0)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Articles_IsDeleted]  DEFAULT ((0)),
	[Status] [int] NOT NULL CONSTRAINT [DF_Articles_Status]  DEFAULT ((1)),
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Articles_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Articles_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_Articles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[ArticleTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArticleId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastModifiedAt] [datetime] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_ArticleTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Bots](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IdentificationCode] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastModifiedAt] [datetime] NOT NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_Bots] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Categories_IsDeleted]  DEFAULT ((0)),
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Categories_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Categories_LastModifiedAt]  DEFAULT (getdate()),
	[Sort] [int] NOT NULL CONSTRAINT [DF_Categories_Sort]  DEFAULT ((50)),
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Comments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArticleId] [int] NOT NULL,
	[CommentContent] [nvarchar](max) NOT NULL,
	[TargetCommentId] [int] NULL,
	[Ip] [nvarchar](50) NULL,
	[UserId] [int] NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_Comments_Status]  DEFAULT ((1)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Comments_IsDeleted]  DEFAULT ((0)),
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Comments_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Comments_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[DailyStatistics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Count] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_DailyStatistics_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_DailyStatistics_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_DailyStatistics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Links](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](100) NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[Target] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastModifiedAt] [datetime] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_Links] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[LogContent] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Logs_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Logs_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Navs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [ParentId] [int] NULL,
	[Text] [nvarchar](100) NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[Target] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL CONSTRAINT [DF_Navs_IsEnabled]  DEFAULT ((1)),
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Navs_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Navs_LastModifiedAt]  DEFAULT (getdate()),
	[Sort] [int] NOT NULL CONSTRAINT [DF_Navs_Sort]  DEFAULT ((50)),
 CONSTRAINT [PK_Navs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Operations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[ReferenceType] [int] NOT NULL,
	[ReferenceId] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Operations_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Operations_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_Operations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Tags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastModifiedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[UserCredentials](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [nvarchar](50) NULL,
	[TokenExpirationTime] [datetime] NULL,
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_UserCredentials_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_UserCredentials_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_UserCredentials] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
    [Status] [int] NOT NULL,
	[Nickname] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Avatar] [nvarchar](max) NULL,
	[Job] [nvarchar](200) NULL,
	[Mobile] [nvarchar](50) NULL,
	[Introduction] [nvarchar](max) NULL,
	[RegIp] [nvarchar](100) NULL,
	[LastLoginedAt] [datetime] NOT NULL CONSTRAINT [DF_Users_LastLoginedAt]  DEFAULT (getdate()),
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Users_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_Users_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[WebConfigs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ConfigKey] [nvarchar](100) NOT NULL,
	[ConfigValue] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_WebConfigs_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_WebConfigs_LastModified]  DEFAULT (getdate()),
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_WebConfigs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[UserInteractions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_UserInteractions_CreatedAt]  DEFAULT (getdate()),
	[LastModifiedAt] [datetime] NOT NULL CONSTRAINT [DF_UserInteractions_LastModifiedAt]  DEFAULT (getdate()),
 CONSTRAINT [PK_UserInteractions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ArticleTags] ADD  CONSTRAINT [DF_ArticleTags_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

ALTER TABLE [dbo].[ArticleTags] ADD  CONSTRAINT [DF_ArticleTags_LastModifiedAt]  DEFAULT (getdate()) FOR [LastModifiedAt]

ALTER TABLE [dbo].[Bots] ADD  CONSTRAINT [DF_Bots_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

ALTER TABLE [dbo].[Bots] ADD  CONSTRAINT [DF_Bots_LastModifiedAt]  DEFAULT (getdate()) FOR [LastModifiedAt]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_IsEnabled]  DEFAULT ((1)) FOR [IsEnabled]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_LastModifiedAt]  DEFAULT (getdate()) FOR [LastModifiedAt]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_Sort]  DEFAULT ((50)) FOR [Sort]

ALTER TABLE [dbo].[Tags] ADD  CONSTRAINT [DF_Tags_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

ALTER TABLE [dbo].[Tags] ADD  CONSTRAINT [DF_Tags_LastModifiedAt]  DEFAULT (getdate()) FOR [LastModifiedAt]

INSERT INTO dbo.Bots (Name, IdentificationCode, Type)
VALUES

-- =============================================
-- 1. SearchEngine 搜索引擎
-- =============================================

(N'Googlebot', N'Googlebot', 1),
(N'Google 图片爬虫', N'Googlebot-Image', 1),
(N'Google 新闻爬虫标识', N'Googlebot-News', 1),
(N'Google 视频爬虫', N'Googlebot-Video', 1),
(N'Google 广告爬虫', N'AdsBot-Google', 1),
(N'Google 移动广告爬虫', N'AdsBot-Google-Mobile', 1),
(N'Google 检查工具', N'Google-InspectionTool', 1),
(N'Google 其他爬虫', N'GoogleOther', 1),
(N'Google 图片其他爬虫', N'GoogleOther-Image', 1),
(N'Google Cloud Vertex 爬虫', N'Google-CloudVertexBot', 1),
(N'Google 商店爬虫', N'Storebot-Google', 1),
(N'Bing 搜索爬虫', N'bingbot', 1),
(N'Bing 广告爬虫', N'AdIdxBot', 1),
(N'Bing 页面预览', N'BingPreview', 1),
(N'百度爬虫', N'Baiduspider', 1),
(N'百度图片爬虫', N'Baiduspider-image', 1),
(N'百度视频爬虫', N'Baiduspider-video', 1),
(N'百度新闻爬虫', N'Baiduspider-news', 1),
(N'百度移动爬虫', N'Baiduspider-mobile', 1),
(N'百度渲染爬虫', N'Baiduspider-render', 1),
(N'360 搜索爬虫', N'360Spider', 1),
(N'搜狗网页爬虫', N'Sogou web spider', 1),
(N'搜狗图片爬虫', N'Sogou Pic Spider', 1),
(N'搜狗新闻爬虫', N'Sogou News Spider', 1),
(N'搜狗微信爬虫', N'Sogou WeChat Spider', 1),
(N'神马搜索爬虫', N'YisouSpider', 1),
(N'Yandex 搜索爬虫', N'YandexBot', 1),
(N'Yandex 图片爬虫', N'YandexImages', 1),
(N'Yandex 视频爬虫', N'YandexVideo', 1),
(N'Yandex 新闻爬虫', N'YandexNews', 1),
(N'Yandex 图片处理爬虫', N'YandexImageResizer', 1),
(N'DuckDuckGo 搜索爬虫', N'DuckDuckBot', 1),
(N'Yahoo 搜索爬虫', N'Slurp', 1),
(N'Apple 搜索爬虫', N'Applebot', 1),
(N'Ask 搜索爬虫', N'Teoma', 1),
(N'Naver 搜索爬虫', N'Yeti', 1),
(N'Seznam 搜索爬虫', N'SeznamBot', 1),
(N'Qwant 搜索爬虫', N'Qwantify', 1),
(N'Mojeek 搜索爬虫', N'MojeekBot', 1),
(N'Ecosia 搜索爬虫', N'Ecosia', 1),
(N'华为 Petal 搜索爬虫', N'PetalBot', 1),
(N'Coc Coc 搜索爬虫', N'coccocbot', 1),
(N'Swisscows 搜索爬虫', N'SwisscowsBot', 1),
(N'Exalead 搜索爬虫', N'Exabot', 1),
(N'AOL 搜索爬虫', N'AOLBuild', 1),

-- =============================================
-- 2. AITraining AI 训练及数据采集
-- =============================================

(N'OpenAI GPT 训练爬虫', N'GPTBot', 2),
(N'Anthropic Claude 训练爬虫', N'ClaudeBot', 2),
(N'Anthropic AI 旧版爬虫', N'anthropic-ai', 2),
(N'Claude 网页爬虫旧版', N'Claude-Web', 2),
(N'字节跳动数据采集爬虫', N'Bytespider', 2),
(N'Common Crawl 公共网页爬虫', N'CCBot', 2),
(N'AI2 研究数据爬虫', N'AI2Bot', 2),
(N'Mistral AI 训练爬虫', N'MistralAI-Training', 2),
(N'Meta AI 外部数据采集', N'meta-externalagent', 2),
(N'Meta 外部数据抓取器', N'meta-externalfetcher', 2),
(N'Google AI 数据使用标识', N'Google-Extended', 2),
(N'Apple AI 数据使用标识', N'Applebot-Extended', 2),
(N'IBM Watson 爬虫', N'IBM-Watson', 2),
(N'Amazon AI 爬虫', N'Amazonbot', 2),
(N'研究数据爬虫', N'ResearchBot', 2),
(N'Webz.io 数据采集爬虫', N'Webzio-Extended', 2),
(N'Cohere AI 爬虫', N'cohere-ai', 2),

-- =============================================
-- 3. AISearch AI 搜索及智能代理
-- =============================================

(N'OpenAI ChatGPT 搜索爬虫', N'OAI-SearchBot', 3),
(N'OpenAI ChatGPT 用户访问', N'ChatGPT-User', 3),
(N'OpenAI 广告审核爬虫', N'OAI-AdsBot', 3),
(N'Anthropic Claude 搜索爬虫', N'Claude-SearchBot', 3),
(N'Anthropic Claude 用户访问', N'Claude-User', 3),
(N'Perplexity AI 搜索爬虫', N'PerplexityBot', 3),
(N'Perplexity 用户访问', N'Perplexity-User', 3),
(N'DuckDuckGo AI 助手', N'DuckAssistBot', 3),
(N'xAI Grok 爬虫', N'GrokBot', 3),
(N'Mistral AI 用户访问', N'MistralAI-User', 3),
(N'You.com AI 搜索爬虫', N'YouBot', 3),
(N'Google AI Agent', N'GoogleAgent-Mariner', 3),
(N'Gemini 相关访问标识', N'Gemini', 3),

-- =============================================
-- 4. SocialMedia 社交媒体
-- =============================================

(N'Facebook 外部分享爬虫', N'facebookexternalhit', 4),
(N'Facebook 爬虫', N'Facebot', 4),
(N'Facebook 相关爬虫', N'FacebookBot', 4),
(N'X Twitter 分享爬虫', N'Twitterbot', 4),
(N'LinkedIn 分享爬虫', N'LinkedInBot', 4),
(N'Pinterest 分享爬虫', N'Pinterestbot', 4),
(N'Pinterest 旧版爬虫', N'Pinterest', 4),
(N'Discord 分享爬虫', N'Discordbot', 4),
(N'Slack 爬虫', N'Slackbot', 4),
(N'Slack 链接预览爬虫', N'Slackbot-LinkExpanding', 4),
(N'Telegram 爬虫', N'TelegramBot', 4),
(N'WhatsApp 分享爬虫', N'WhatsApp', 4),
(N'Skype 链接预览爬虫', N'SkypeUriPreview', 4),
(N'Microsoft Teams 爬虫', N'TeamsBot', 4),
(N'Flipboard 爬虫', N'FlipboardProxy', 4),
(N'Reddit 爬虫', N'Redditbot', 4),
(N'Tumblr 爬虫', N'Tumblr', 4),
(N'微信客户端标识', N'MicroMessenger', 4),

-- =============================================
-- 5. SeoTools SEO 工具
-- =============================================

(N'Ahrefs 爬虫', N'AhrefsBot', 5),
(N'Ahrefs 网站审计爬虫', N'AhrefsSiteAudit', 5),
(N'Semrush 爬虫', N'SemrushBot', 5),
(N'Semrush 网站审计爬虫', N'SiteAuditBot', 5),
(N'Semrush BA 爬虫', N'SemrushBot-BA', 5),
(N'Semrush SI 爬虫', N'SemrushBot-SI', 5),
(N'Semrush SWA 爬虫', N'SemrushBot-SWA', 5),
(N'Majestic SEO 爬虫', N'MJ12bot', 5),
(N'Moz SEO 爬虫', N'DotBot', 5),
(N'Moz Rogerbot 爬虫', N'rogerbot', 5),
(N'Screaming Frog SEO 爬虫', N'Screaming Frog SEO Spider', 5),
(N'Screaming Frog 爬虫', N'ScreamingFrogSEOSpider', 5),
(N'DataForSEO 爬虫', N'DataForSeoBot', 5),
(N'SE Ranking 爬虫', N'SEBot', 5),
(N'SE Ranking 旧版爬虫', N'SE Ranking Bot', 5),
(N'OnCrawl SEO 爬虫', N'OnCrawl', 5),
(N'Botify SEO 爬虫', N'Botify', 5),
(N'DeepCrawl SEO 爬虫', N'DeepCrawl', 5),
(N'Sitebulb SEO 爬虫', N'Sitebulb', 5),
(N'SISTRIX SEO 爬虫', N'SISTRIX Crawler', 5),
(N'Searchmetrics SEO 爬虫', N'SearchmetricsBot', 5),
(N'SpyFu SEO 爬虫', N'SpyFu', 5),
(N'Serpstat SEO 爬虫', N'SerpstatBot', 5),
(N'Netpeak SEO 爬虫', N'NetpeakCheckerBot', 5),
(N'Xenu 链接检查工具', N'Xenu Link Sleuth', 5),
(N'LinkResearchTools 爬虫', N'LinkResearchTools', 5),
(N'Audisto SEO 爬虫', N'Audisto', 5),
(N'Babbar SEO 爬虫', N'Barkrowler', 5),
(N'Attracta SEO 爬虫', N'Attracta', 5),
(N'Sitechecker SEO 爬虫', N'SitecheckerBot', 5),

-- =============================================
-- 6. WebsiteTools 网站监控及检测工具
-- =============================================

(N'Internet Archive 网页存档', N'ia_archiver', 6),
(N'Internet Archive 爬虫', N'archive.org_bot', 6),
(N'UptimeRobot 网站监控', N'UptimeRobot', 6),
(N'Pingdom 网站监控', N'Pingdom', 6),
(N'StatusCake 网站监控', N'StatusCake', 6),
(N'Site24x7 网站监控', N'Site24x7', 6),
(N'Better Uptime 网站监控', N'Better Uptime', 6),
(N'Datadog 自动化监控', N'DatadogSynthetics', 6),
(N'New Relic 网站监控', N'NewRelicPinger', 6),
(N'Sematext 网站监控', N'Sematext', 6),
(N'Monitis 网站监控', N'Monitis', 6),
(N'Checkly 网站监控', N'Checkly', 6),
(N'GTmetrix 网站检测', N'GTmetrix', 6),
(N'Google PageSpeed 网站检测', N'Google Page Speed Insights', 6),
(N'WebPageTest 网站检测', N'WebPageTest', 6),
(N'W3C 链接检查工具', N'W3C-checklink', 6),
(N'W3C HTML 验证工具', N'W3C_Validator', 6),
(N'W3C CSS 验证工具', N'W3C_CSS_Validator', 6),
(N'Feedly RSS 订阅爬虫', N'Feedly', 6),
(N'Feedbin RSS 订阅爬虫', N'Feedbin', 6),
(N'Inoreader RSS 订阅爬虫', N'Inoreader', 6),
(N'NewsBlur RSS 订阅爬虫', N'NewsBlur', 6),
(N'Netvibes RSS 订阅爬虫', N'Netvibes', 6),
(N'BazQux RSS 订阅爬虫', N'BazQux', 6),
(N'Google Feed 订阅爬虫', N'Feedfetcher-Google', 6),
(N'Yahoo Feed 订阅爬虫', N'YahooFeedSeeker', 6),
(N'Archive Team 网页存档', N'ArchiveTeam', 6),

-- =============================================
-- 7. SecurityScanner 安全扫描工具
-- =============================================

(N'Nessus 漏洞扫描', N'Nessus', 7),
(N'OpenVAS 漏洞扫描', N'OpenVAS', 7),
(N'Qualys 安全扫描', N'Qualys', 7),
(N'Acunetix 漏洞扫描', N'Acunetix', 7),
(N'Burp Suite 安全扫描', N'Burp', 7),
(N'OWASP ZAP 安全扫描', N'ZAP', 7),
(N'Nikto 网站扫描', N'Nikto', 7),
(N'Nmap 网络扫描', N'Nmap Scripting Engine', 7),
(N'sqlmap SQL 注入扫描', N'sqlmap', 7),
(N'WPScan WordPress 扫描', N'WPScan', 7),
(N'Arachni 网站扫描', N'Arachni', 7),
(N'w3af 安全扫描', N'w3af', 7),
(N'Netsparker 漏洞扫描', N'Netsparker', 7),
(N'Rapid7 安全扫描', N'InsightVM', 7),
(N'Censys 网络扫描', N'CensysInspect', 7),
(N'Shodan 网络扫描', N'Shodan', 7),
(N'zgrab 网络扫描', N'zgrab', 7),

-- =============================================
-- 8. AutomationTools 自动化客户端工具
-- =============================================

(N'curl HTTP 客户端', N'curl/', 8),
(N'wget HTTP 下载工具', N'Wget/', 8),
(N'Python Requests', N'python-requests', 8),
(N'Python urllib', N'Python-urllib', 8),
(N'Python HTTPX', N'python-httpx', 8),
(N'Python aiohttp', N'Python/aiohttp', 8),
(N'Scrapy 爬虫框架', N'Scrapy', 8),
(N'Go HTTP 客户端', N'Go-http-client', 8),
(N'Node.js Fetch', N'node-fetch', 8),
(N'Axios HTTP 客户端', N'axios/', 8),
(N'Java HTTP 客户端', N'Java/', 8),
(N'OkHttp HTTP 客户端', N'okhttp/', 8),
(N'Apache HTTP 客户端', N'Apache-HttpClient', 8),
(N'PHP HTTP 客户端', N'PHP/', 8),
(N'Guzzle HTTP 客户端', N'GuzzleHttp', 8),
(N'Ruby HTTP 客户端', N'Ruby', 8),
(N'Mechanize 自动化客户端', N'Mechanize', 8),
(N'Perl LWP HTTP 客户端', N'libwww-perl', 8),
(N'Postman API 测试工具', N'PostmanRuntime', 8),
(N'Insomnia API 测试工具', N'insomnia/', 8),
(N'HTTPie HTTP 客户端', N'HTTPie/', 8),
(N'Puppeteer 浏览器自动化', N'Puppeteer', 8),
(N'Playwright 浏览器自动化', N'Playwright', 8),
(N'Selenium 浏览器自动化', N'Selenium', 8),
(N'PhantomJS 无头浏览器', N'PhantomJS', 8),
(N'Headless Chrome 无头浏览器', N'HeadlessChrome', 8),
(N'Chrome Lighthouse 性能检测', N'Chrome-Lighthouse', 8),
(N'Diffbot 网页数据提取', N'Diffbot', 8),
(N'Octoparse 网页采集工具', N'Octoparse', 8),
(N'Apify 网页采集工具', N'Apify', 8),
(N'Bright Data 网页采集工具', N'BrightData', 8),
(N'Google Mariner Agent', N'GoogleAgent-Mariner', 8),
(N'OpenAI Operator Agent', N'Operator', 8),
(N'Cognition Devin Agent', N'Devin', 8),
(N'browser-use 自动化框架', N'browser-use', 8),
(N'LangChain 自动化框架', N'LangChain', 8),
(N'LlamaIndex 自动化框架', N'LlamaIndex', 8),
(N'AutoGPT 自动化框架', N'AutoGPT', 8),
(N'Cloudflare Always Online', N'CloudFlare-AlwaysOnline', 8),
(N'Cloudflare 健康检查', N'CloudFlare-Healthchecks', 8),
(N'Cloudflare 流量管理检查', N'CloudFlare-Traffic-Manager', 8),
(N'Cloudflare 浏览器渲染爬虫', N'CloudflareBrowserRenderingCrawler', 8),
(N'Cloudflare AI Search', N'Cloudflare-AI-Search', 8),
(N'Cloudflare 域名验证', N'Cloudflare Custom Hostname Verification', 8);
";
    }
}
