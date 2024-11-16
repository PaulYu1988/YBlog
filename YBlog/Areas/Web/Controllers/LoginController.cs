using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;
using YBlog.Models.Custom;
using YBlog.Models.Enums;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class LoginController : Controller
    {
        protected IUserService _userService;
        protected IOperationService _operationService;
        public LoginController(IUserService userService, IOperationService operationService)
        {
            _userService = userService;
            _operationService = operationService;
        }
        [HttpGet]
        public IActionResult Index(string? from)
        {
            if (!string.IsNullOrWhiteSpace(from))
            {
                Request.HttpContext.Session.SetString(EnumSessionNames.From.ToString(), from);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var captchaSessionName = EnumSessionNames.Captcha.ToString();
            var captcha = Request.HttpContext.Session.GetString(captchaSessionName);
            Request.HttpContext.Session.Remove(captchaSessionName);
            if (!string.Equals(captcha, request.Captcha, StringComparison.OrdinalIgnoreCase))
            {
                return this.Error(EnumErrorCodes.CaptchaError);
            }
            request.Password = request.Password.ToMD5();
            var token = Guid.NewGuid().ToString();
            var userCredential = await _userService.LoginAsync(request, token.ToMD5());
            if (userCredential == null)
            {
                return this.Error(EnumErrorCodes.IncorrectAccountOrPassword);
            }
            var user = await _userService.UpdateLastLoginedAtAsync(userCredential.UserId);
            if (user == null)
            {
                return this.InternalServerError();
            }
            if (user.Status == (int)EnumUserStatuses.Disabled)
            {
                return this.Error(EnumErrorCodes.UserIsDisabled);
            }
            var userState = new UserState()
            {
                IsLogin = true,
                Username = userCredential.Username,
                Id = userCredential.UserId,
                Email = user.Email,
                Avatar = user.Avatar,
                Nickname = user.Nickname,
                Type = (EnumUserTypes)user.Type,
                Status = (EnumUserStatuses)user.Status
            };
            if (userState.Type == EnumUserTypes.Admin)
            {
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Login, EnumOperationReferenceTypes.User, null, $"{userCredential.Username} {Request.GetRequestIpAddress()}");
            }
            Request.HttpContext.Session.Set(EnumSessionNames.UserState.ToString(), userState);
            if (request.StayLogin)
            {
                Request.HttpContext.Response.Cookies.Append(EnumCookieNames.Token.ToString(), token, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30)
                });
            }
            else
            {
                Request.HttpContext.Response.Cookies.Append(EnumCookieNames.Token.ToString(), string.Empty, new CookieOptions
                {
                    Expires = DateTime.Now
                });
            }
            var from = Request.HttpContext.Session.GetString(EnumSessionNames.From.ToString());
            if (!string.IsNullOrWhiteSpace(from))
                Request.HttpContext.Session.SetString(EnumSessionNames.From.ToString(), string.Empty);
            return this.Success(new
            {
                from
            });
        }
    }
}
