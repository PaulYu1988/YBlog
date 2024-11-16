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

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_IsEnabled]  DEFAULT ((1)) FOR [IsEnabled]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_LastModifiedAt]  DEFAULT (getdate()) FOR [LastModifiedAt]

ALTER TABLE [dbo].[Links] ADD  CONSTRAINT [DF_Links_Sort]  DEFAULT ((50)) FOR [Sort]

ALTER TABLE [dbo].[Tags] ADD  CONSTRAINT [DF_Tags_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

ALTER TABLE [dbo].[Tags] ADD  CONSTRAINT [DF_Tags_LastModifiedAt]  DEFAULT (getdate()) FOR [LastModifiedAt]
";
    }
}
