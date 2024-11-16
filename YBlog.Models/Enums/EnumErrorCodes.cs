using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumErrorCodes
    {
        [Description("验证码错误")]
        CaptchaError = 1,
        [Description("账户或密码错误")]
        IncorrectAccountOrPassword = 2,
        [Description("用户名已存在")]
        UsernameExist = 3,
        [Description("密码错误")]
        IncorrectPassword = 4,
        [Description("昵称已存在")]
        NicknameExist = 5,
        [Description("用户被禁用")]
        UserIsDisabled = 6
    }
}
