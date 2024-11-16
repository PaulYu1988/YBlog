using Microsoft.AspNetCore.Mvc;
using YBlog.Models.Custom;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Services;
using YBlog.Models.Enums;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class FilesController : Controller
    {
        protected IWebHostEnvironment _env;
        protected IOperationService _operationService;
        public FilesController(IWebHostEnvironment env, IOperationService operationService)
        {
            _env = env;
            _operationService = operationService;
        }
        [HttpGet]
        public IActionResult Index(string? path)
        {
            ViewBag.Path = path;
            var fileItems = new List<FileItem>();
            var directoryItems = new List<DirectoryItem>();
            var filePath = _env.WebRootPath;
            var currPath = string.IsNullOrWhiteSpace(path) ? filePath : filePath + path;
            if (Directory.Exists(currPath))
            {
                ViewBag.Exists = true;
                var currentDirectoryInfo = new DirectoryInfo(currPath);
                var parentDirectoryInfo = currentDirectoryInfo.Parent;
                var parent = parentDirectoryInfo?.FullName ?? string.Empty;
                if (parent.Contains(filePath))
                {
                    parent = parent.Replace(filePath, string.Empty);
                }
                else
                {
                    parent = string.Empty;
                }
                ViewBag.Parent = parent;
                var files = Directory.GetFiles(currPath);
                foreach (var f in files)
                {
                    var fileInfo = new FileInfo(f);
                    var fileItem = new FileItem()
                    {
                        Name = fileInfo.Name,
                        Extension = fileInfo.Extension.ToLower(),
                        RelativePath = f.Replace(filePath, string.Empty),
                        CreationTime = fileInfo.CreationTime
                    };
                    fileItems.Add(fileItem);
                }
                ViewBag.FileItems = fileItems.OrderByDescending(x => x.CreationTime).ToList();
                var directories = Directory.GetDirectories(currPath);
                foreach (var d in directories)
                {
                    var directoryInfo = new DirectoryInfo(d);
                    var directoryItem = new DirectoryItem()
                    {
                        Name = directoryInfo.Name,
                        RelativePath = d.Replace(filePath, string.Empty),
                        CreationTime = directoryInfo.CreationTime
                    };
                    directoryItems.Add(directoryItem);
                }
                ViewBag.DirectoryItems = directoryItems.OrderByDescending(x => x.CreationTime).ToList(); ;
            }
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }
            var userState = Request.GetUserState();
            var filePath = _env.WebRootPath;
            var currPath = filePath + id;
            if (System.IO.File.Exists(currPath))
            {
                try
                {
                    System.IO.File.Delete(currPath);
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.File, null, id);
                    return this.Success();
                }
                catch (Exception ex)
                {
                    return this.InternalServerError(ex);
                }
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
