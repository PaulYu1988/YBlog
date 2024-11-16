using YBlog.Models.Database;
using Microsoft.EntityFrameworkCore;
using YBlog.Services;
using YBlog.Services.Implement;
using YBlog.Services.Mock;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<YBlogContext>(opt =>
{
    var compatibilityLevel = builder.Configuration.GetValue<int?>("CompatibilityLevel");
    if (compatibilityLevel.HasValue && compatibilityLevel.Value < 130)
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("YBlog"),
            o => o.UseCompatibilityLevel(120));
    }
    else
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("YBlog"));
    }
});
var mockEnabled = builder.Configuration.GetValue<bool>("MockEnabled");
if (mockEnabled)
{
    builder.Services.AddScoped<IArticleService, MockArticleService>();
    builder.Services.AddScoped<ICategoryService, MockCategoryService>();
    builder.Services.AddScoped<ICommentService, MockCommentService>();
    builder.Services.AddScoped<ITagService, MockTagService>();
    builder.Services.AddScoped<IUserService, MockUserService>();
    builder.Services.AddScoped<INavService, MockNavService>();
    builder.Services.AddScoped<IWebConfigService, MockWebConfigService>();
    builder.Services.AddScoped<ICacheService, MockCacheService>();
    builder.Services.AddScoped<ILinkService, MockLinkService>();
    builder.Services.AddScoped<IAdvertisementService, MockAdvertisementService>();
    builder.Services.AddScoped<IOperationService, MockOperationService>();
    builder.Services.AddScoped<ILogService, MockLogService>();
    builder.Services.AddScoped<IStatisticService, MockStatisticService>();
    builder.Services.AddScoped<IUserInteractionService, MockUserInteractionService>();
}
else
{
    builder.Services.AddScoped<IArticleService, ArticleService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<INavService, NavService>();
    builder.Services.AddScoped<IWebConfigService, WebConfigService>();
    builder.Services.AddScoped<ICacheService, CacheService>();
    builder.Services.AddScoped<ILinkService, LinkService>();
    builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
    builder.Services.AddScoped<IOperationService, OperationService>();
    builder.Services.AddScoped<ILogService, LogService>();
    builder.Services.AddScoped<IStatisticService, StatisticService>();
    builder.Services.AddScoped<IUserInteractionService, UserInteractionService>();
}

var app = builder.Build();
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/home/error/{0}");
// app.UseStatusCodePagesWithRedirects("/home/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapAreaControllerRoute(name: "Web", areaName: "Web", pattern: "{controller}/{action}/{id?}", new { area = "Web", controller = "Home", action = "Index" });
app.MapAreaControllerRoute(name: "Web_Home", areaName: "Web", pattern: "", new { area = "Web", controller = "Home", action = "Index" });
app.MapAreaControllerRoute(name: "Web_Install", areaName: "Web", pattern: "", new { area = "Web", controller = "Install", action = "Index" });
app.MapAreaControllerRoute(name: "Web_Articles", areaName: "Web", pattern: "articles/{id?}", new { controller = "Articles", action = "Index" });
app.MapAreaControllerRoute(name: "Web_Categories", areaName: "Web", pattern: "categories/{id?}", new { controller = "Categories", action = "Index" });
app.MapAreaControllerRoute(name: "Web_Search", areaName: "Web", pattern: "search/{id?}", new { controller = "Search", action = "Index" });
app.MapAreaControllerRoute(name: "Web_Tags", areaName: "Web", pattern: "tags/{id?}", new { controller = "Tags", action = "Index" });
app.MapAreaControllerRoute(name: "Manage", areaName: "Manage", pattern: "{area}/{controller}/{action}/{id?}", new { area = "Manage", controller = "Home", action = "Index" });
app.MapAreaControllerRoute(name: "Common", areaName: "Common", pattern: "{area}/{controller}/{action}/{id?}", new { area = "Common", controller = "Home", action = "Index" });
app.MapAreaControllerRoute(name: "Account", areaName: "Account", pattern: "{area}/{controller}/{action}/{id?}", new { area = "Account", controller = "Home", action = "Index" });
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var logService = services.GetRequiredService<ILogService>();
    logService.Log(YBlog.Models.Enums.EnumLogTypes.Info, "application start");
}
app.Run();
