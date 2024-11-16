using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumOperationTypes
    {
        [Description("登录")]
        Login = 1,
        [Description("添加")]
        Create = 2,
        [Description("更新")]
        Update = 3,
        [Description("删除")]
        Delete = 4,
    }
}
