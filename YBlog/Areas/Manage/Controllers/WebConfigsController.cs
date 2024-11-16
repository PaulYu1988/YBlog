using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Custom;
using YBlog.Models.Enums;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class WebConfigsController : Controller
    {
        protected IWebHostEnvironment _env;
        protected ICacheService _cacheService;
        protected IWebConfigService _webConfigService;
        protected IOperationService _operationService;
        public WebConfigsController(IWebHostEnvironment env, ICacheService cacheService, IWebConfigService webConfigService, IOperationService operationService)
        {
            _env = env;
            _cacheService = cacheService;
            _webConfigService = webConfigService;
            _operationService = operationService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var items = await _webConfigService.GetAllAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(WebConfigView webConfig)
        {
            var userState = Request.GetUserState();
            if (webConfig == null)
            {
                return this.BadRequest();
            }
            var files = new Dictionary<string, string>();
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    var name = file.Name;
                    var extension = Path.GetExtension(file.FileName);
                    var newFileName = $"{DateTime.Now.ToString("HHmmssfff")}{extension}";
                    var path = $"{DateTime.Now.ToString("yyyyMMdd")}";
                    var url = $"/upload/{path}/{userState.Id}/{newFileName}";
                    var filePath = $"{_env.WebRootPath}\\upload\\{path}\\{userState.Id}";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    using (FileStream filestream = System.IO.File.Create($"{filePath}\\{newFileName}"))
                    {
                        await file.CopyToAsync(filestream);
                        filestream.Flush();
                    }
                    files.Add(name, url);
                }
            }
            var result = await _webConfigService.UpdateAsync(webConfig, files);
            if (result)
            {
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.WebConfig);
                _cacheService.Clear(EnumCacheNames.WebConfigs);
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
