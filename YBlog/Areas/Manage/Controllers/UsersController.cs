using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services;
using YBlog.Models.Enums;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class UsersController : Controller
    {
        protected IUserService _userService;
        protected IOperationService _operationService;
        protected IWebHostEnvironment _env;
        public UsersController(IUserService userService, IOperationService operationService, IWebHostEnvironment env)
        {
            _userService = userService;
            _operationService = operationService;
            _env = env;
        }
        public async Task<IActionResult> IndexAsync(UserPagedQuery query)
        {
            var items = await _userService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {
            var userRequest = new UserRequest();
            if (id.HasValue)
            {
                var item = await _userService.GetByIdAsync(id.Value);
                if (item != null)
                {
                    userRequest.Id = item.Id;
                    userRequest.Nickname = item.Nickname;
                    userRequest.Avatar = item.Avatar;
                    userRequest.Introduction = item.Introduction;
                    userRequest.Email = item.Email;
                    userRequest.Job = item.Job;
                    userRequest.Mobile = item.Mobile;
                    userRequest.Type = item.Type;
                    userRequest.Status = item.Status;
                }
            }
            return View(userRequest);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(UserRequest request)
        {
            if (!ModelState.IsValid)
                return this.BadRequest();
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
            var result = await _userService.UpdateAsync(request);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.User, request.Id);
                if (userState.Id == request.Id)
                {
                    Request.RefreshUserState();
                }
                return this.Success(request);
            }
            return this.InternalServerError();
        }
    }
}
