using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;

namespace YBlog.Services
{
    public interface ICacheService
    {
        List<Nav> GetNavs();
        List<Link> GetLinks();
        WebConfigView GetWebConfigView();
        List<Article> GetTopArticles(int take = 8);
        List<Tag> GetTopTags(int take = 32);
        void Clear(EnumCacheNames cacheName);
        List<ManageMenu> GetManageMenus(string path);
        List<Advertisement> GetAdvertisements();
        string GetAssetsUrl(string path);
        string GetAbbreviation(string? nickname);
        string GetAvatarBackgroundColor(int? userId);
    }
}
