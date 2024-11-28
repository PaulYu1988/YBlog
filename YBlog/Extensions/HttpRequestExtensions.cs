using YBlog.Models.Enums;
using YBlog.Models.Database;
using YBlog.Models.Custom;

namespace YBlog.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetRequestIpAddress(this HttpRequest httpRequest)
        {
            var remoteIpAddress = httpRequest.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(remoteIpAddress))
            {
                remoteIpAddress = httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            return remoteIpAddress ?? string.Empty;
        }
        public static UserState GetUserState(this HttpRequest httpRequest)
        {
            var userState = httpRequest.HttpContext.Session.Get<UserState>(EnumSessionNames.UserState.ToString());
            if (userState != null)
            {
                return userState;
            }
            userState = new UserState();
            var config = httpRequest.HttpContext.RequestServices.GetService<IConfiguration>();
            if (config != null && config.GetValue<bool>("MockEnabled"))
            {
                userState.IsLogin = true;
                userState.Id = 1;
                userState.Nickname = "Nickname";
                userState.Type = EnumUserTypes.Admin;
                userState.Status = EnumUserStatuses.Enabled;
                userState.Username = "Username";
                userState.Email = "yy@yy.yy";
            }
            else
            {
                var token = httpRequest.Cookies[EnumCookieNames.Token.ToString()];
                if (string.IsNullOrWhiteSpace(token))
                    return userState;
                var context = httpRequest.HttpContext.RequestServices.GetService<YBlogContext>();
                var userCredential = context?.UserCredentials.FirstOrDefault(x => x.Token == token.ToMD5() && x.TokenExpirationTime > DateTime.Now);
                if (userCredential == null)
                    return userState;
                var user = context?.Users.FirstOrDefault(x => x.Id == userCredential.UserId);
                if (user == null)
                    return userState;
                if (user.Status == (int)EnumUserStatuses.Disabled)
                {
                    return userState;
                }
                user.LastLoginedAt = DateTime.Now;
                context?.SaveChanges();
                userState.IsLogin = true;
                userState.Id = user.Id;
                userState.Avatar = user.Avatar;
                userState.Nickname = user.Nickname;
                userState.Type = (EnumUserTypes)user.Type;
                userState.Status = (EnumUserStatuses)user.Status;
                userState.Username = userCredential.Username;
                userState.Email = user.Email;
            }
            httpRequest.HttpContext.Session.Set(EnumSessionNames.UserState.ToString(), userState);
            return userState;
        }
        public static bool IsAjax(this HttpRequest httpRequest)
        {
            if (httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return true;
            }
            return false;
        }

        public static void RefreshUserState(this HttpRequest httpRequest)
        {
            var userState = httpRequest.HttpContext.Session.Get<UserState>(EnumSessionNames.UserState.ToString());
            if (userState != null)
            {
                var context = httpRequest.HttpContext.RequestServices.GetService<YBlogContext>();
                if (context != null)
                {
                    var user = context.Users.FirstOrDefault(x => x.Id == userState.Id);
                    if (user != null)
                    {
                        userState.Avatar = user.Avatar;
                        userState.Nickname = user.Nickname;
                        userState.Email = user.Email;
                        httpRequest.HttpContext.Session.Set(EnumSessionNames.UserState.ToString(), userState);
                    }
                }
            }
        }
        public static EnumThemeModes GetThemeMode(this HttpRequest httpRequest)
        {
            var result = EnumThemeModes.Light;
            var themeMode = httpRequest.Cookies[EnumCookieNames.ThemeMode.ToString()];
            if (!string.IsNullOrWhiteSpace(themeMode))
            {
                if (themeMode == "dark")
                {
                    result = EnumThemeModes.Dark;
                }
            }
            return result;
        }
    }
}
