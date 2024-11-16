using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;

namespace YBlog.Areas.Common.Controllers
{
    [Area("Common")]
    public class UploadController : Controller
    {
        protected IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync()
        {
            var userState = Request.GetUserState();
            if (!userState.IsLogin)
                return this.Unauthorized();
            if (userState.Type == Models.Enums.EnumUserTypes.Common)
                return this.Forbid();
            if (!Request.Form.Files.Any())
            {
                return this.BadRequest();
            }
            var file = Request.Form.Files[0];
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                return this.BadRequest();
            }
            extension = extension.ToLower();
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            if (!allowedExtensions.Contains(extension))
            {
                return this.BadRequest();
            }
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();
                    if (!IsImage(bytes))
                    {
                        return this.BadRequest();
                    }
                }
                var newFileName = $"{DateTime.Now.ToString("HHmmssfff")}{extension}";
                var path = $"{DateTime.Now.ToString("yyyyMMdd")}";
                var url = $"/upload/{path}/{newFileName}";
                var filePath = $"{_env.WebRootPath}\\upload\\{path}";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (FileStream filestream = System.IO.File.Create($"{filePath}\\{newFileName}"))
                {
                    await file.CopyToAsync(filestream);
                    filestream.Flush();
                }
                return this.Success(new { location = url });
            }
            catch
            {
                return this.InternalServerError();
            }
        }
        private bool IsImage(byte[] data)
        {
            // 检查 JPEG 文件头
            if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF && data[3] == 0xE0)
            {
                return true;
            }

            // 检查 PNG 文件头
            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47 && data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A)
            {
                return true;
            }

            // 检查 GIF 文件头
            if (data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38 && data[4] == 0x37 && data[5] == 0x61)
            {
                return true;
            }

            // 检查 BMP 文件头
            if (data[0] == 0x42 && data[1] == 0x4D)
            {
                return true;
            }

            return false;
        }
    }
}
