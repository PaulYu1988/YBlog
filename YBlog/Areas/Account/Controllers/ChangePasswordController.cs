using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Requests;
using YBlog.Services;
using YBlog.Models.Enums;

namespace YBlog.Areas.Account.Controllers
{
    [Area("Account")]
    [PostLogin]
    public class ChangePasswordController : Controller
    {
        protected IUserService _userService;
        public ChangePasswordController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return this.BadRequest();
            var userState = Request.GetUserState();
            var userCredential = await _userService.GetUserCredentialById(userState.Id);
            if (userCredential?.Password != request.Password.ToMD5())
                return this.Error(EnumErrorCodes.IncorrectPassword);
            var result = await _userService.ChangePasswordAsync(userState.Id, request.NewPassword.ToMD5());
            return result ? this.Success() : this.InternalServerError();
        }
    }
}
