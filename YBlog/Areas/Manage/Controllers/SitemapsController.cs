using Microsoft.AspNetCore.Mvc;
using System.Text;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Services;
using YBlog.Models.Enums;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class SitemapsController : Controller
    {
        protected IWebHostEnvironment _env;
        protected ICacheService _cacheService;
        protected ICategoryService _categoryService;
        protected IArticleService _articleService;
        protected IOperationService _operationService;
        public SitemapsController(IWebHostEnvironment env, ICacheService cacheService, ICategoryService categoryService, IArticleService articleService, IOperationService operationService)
        {
            _env = env;
            _cacheService = cacheService;
            _categoryService = categoryService;
            _articleService = articleService;
            _operationService = operationService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var xmlPath = $"{_env.WebRootPath}/sitemap.xml";
            var txtPath = $"{_env.WebRootPath}/sitemap.txt";
            FileInfo xmlFileInfo = new FileInfo(xmlPath);
            FileInfo txtFileInfo = new FileInfo(txtPath);
            ViewBag.XmlFileInfo = xmlFileInfo;
            ViewBag.TxtFileInfo = txtFileInfo;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync()
        {
            var webConfigs = _cacheService.GetWebConfigView();
            var SiteAddress = webConfigs.SiteAddress ?? $"{Request.Scheme}://{Request.Host}";
            string path = $"{_env.WebRootPath}";
            StringBuilder sb = new StringBuilder();
            StringBuilder txtSb = new StringBuilder();
            txtSb.AppendLine(SiteAddress);
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            sb.Append("<url>");
            sb.AppendFormat("<loc>{0}</loc>", SiteAddress);
            sb.AppendFormat("<lastmod>{0}</lastmod>", DateTime.Now.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<changefreq>{0}</changefreq>", Changefreq.always);
            sb.AppendFormat("<priority>{0}</priority>", "1.0");
            sb.Append("</url>");
            var clist = await _categoryService.GetAllAsync();
            if (clist.Any())
            {
                foreach (var c in clist)
                {
                    var cUrl = SiteAddress + Url.Content("~/categories/" + c.Id);
                    txtSb.AppendLine(cUrl);
                    sb.Append("<url>");
                    sb.AppendFormat("<loc>{0}</loc>", cUrl);
                    sb.AppendFormat("<lastmod>{0}</lastmod>", DateTime.Now.ToString("yyyy-MM-dd"));
                    sb.AppendFormat("<changefreq>{0}</changefreq>", Changefreq.daily);
                    sb.AppendFormat("<priority>{0}</priority>", "0.9");
                    sb.Append("</url>");
                }
            }
            var alist = await _articleService.GetAllAsync();
            if (alist.Any())
            {
                foreach (var a in alist)
                {
                    var aUrl = SiteAddress + Url.Content("~/articles/" + a.Id);
                    txtSb.AppendLine(aUrl);
                    sb.Append("<url>");
                    sb.AppendFormat("<loc>{0}</loc>", aUrl);
                    sb.AppendFormat("<lastmod>{0}</lastmod>", a.LastModifiedAt.ToString("yyyy-MM-dd"));
                    sb.AppendFormat("<changefreq>{0}</changefreq>", Changefreq.weekly);
                    sb.AppendFormat("<priority>{0}</priority>", "0.8");
                    sb.Append("</url>");
                }
            }

            sb.Append("</urlset>");
            try
            {
                using (StreamWriter sw = new StreamWriter(path + "/sitemap.xml", false))
                {
                    sw.Write(sb.ToString());
                }
                using (StreamWriter sw = new StreamWriter(path + "/sitemap.txt", false, Encoding.UTF8))
                {
                    sw.Write(txtSb.ToString());
                }
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Create, EnumOperationReferenceTypes.Sitemap);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(new
                {
                    ex.Message
                });
            }
            return this.Success();
        }
        public enum Changefreq
        {
            always,
            hourly,
            daily,
            weekly,
            monthly,
            yearly,
            never
        }
    }
}
