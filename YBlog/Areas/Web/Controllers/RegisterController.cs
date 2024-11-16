using Microsoft.AspNetCore.Mvc;
using YBlog.Extensions;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Web.Controllers
{
    [Area("Web")]
    public class RegisterController : Controller
    {
        protected IUserService _userService;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(RegisterRequest request)
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
            if (string.IsNullOrWhiteSpace(request.Username) || (await _userService.IsUsernameExistAsync(request.Username)))
            {
                return this.Error(EnumErrorCodes.UsernameExist);
            }
            var user = new User()
            {
                Nickname = request.Nickname ?? request.Username,
                Type = (int)EnumUserTypes.Common,
                Status = (int)EnumUserStatuses.Enabled,
                RegIp = Request.HttpContext.Request.GetRequestIpAddress()
            };
            var token = Guid.NewGuid().ToString();
            var userCredential = new UserCredential()
            {
                Password = request.Password.ToMD5(),
                Token = token.ToMD5(),
                TokenExpirationTime = DateTime.Now.AddDays(30),
                Username = request.Username
            };
            var result = await _userService.RegisterAsync(user, userCredential);
            if (!result)
            {
                return this.InternalServerError();
            }
            var userState = new UserState()
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Type = EnumUserTypes.Common,
                Status = EnumUserStatuses.Enabled,
                Username = userCredential.Username,
                IsLogin = true
            };
            Request.HttpContext.Session.Set(EnumSessionNames.UserState.ToString(), userState);
            if (request.StayLogin)
            {
                Request.HttpContext.Response.Cookies.Append(EnumCookieNames.Token.ToString(), token, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30)
                });
            }
            var from = Request.HttpContext.Session.GetString(EnumSessionNames.From.ToString());
            if (!string.IsNullOrWhiteSpace(from))
                Request.HttpContext.Session.SetString(EnumSessionNames.From.ToString(), string.Empty);
            return this.Success(new { from });
        }
    }
}
