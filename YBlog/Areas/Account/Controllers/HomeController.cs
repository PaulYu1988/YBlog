using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Enums;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Account.Controllers
{
    [Area("Account")]
    [PostLogin]
    public class HomeController : Controller
    {
        protected IUserService _userService;
        protected IWebHostEnvironment _env;
        public HomeController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userState = Request.GetUserState();
            var item = await _userService.GetByIdAsync(userState.Id);
            if (item == null)
                return Redirect("/login");
            var userRequest = new UserRequest();
            userRequest.Id = item.Id;
            userRequest.Nickname = item.Nickname;
            userRequest.Avatar = item.Avatar;
            userRequest.Introduction = item.Introduction;
            userRequest.Email = item.Email;
            userRequest.Job = item.Job;
            userRequest.Mobile = item.Mobile;
            userRequest.Type = item.Type;
            userRequest.Status = item.Status;
            return View(userRequest);
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(UserRequest request)
        {
            if (!ModelState.IsValid)
                return this.BadRequest();
            var userState = Request.GetUserState();
            request.Id = userState.Id;
            if (string.IsNullOrWhiteSpace(request.Nickname) || (await _userService.IsNicknameExistAsync(request.Nickname, userState.Id)))
            {
                return this.Error(EnumErrorCodes.NicknameExist);
            }
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];
                var extension = Path.GetExtension(file.FileName);
                var newFileName = $"{request.Id}-{DateTime.Now.ToString("HHmmssfff")}{extension}";
                var url = $"/upload/avatars/{newFileName}";
                var filePath = $"{_env.WebRootPath}\\upload\\avatars";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (FileStream filestream = System.IO.File.Create($"{filePath}\\{newFileName}"))
                {
                    await file.CopyToAsync(filestream);
                    filestream.Flush();
                }
                request.Avatar = url;
            }
            request.Type = null;
            request.Status = null;
            var result = await _userService.UpdateAsync(request);
            if (result)
            {
                Request.RefreshUserState();
                return this.Success(request);
            }
            return this.InternalServerError();
        }
    }
}
