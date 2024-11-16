using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumUserTypes
    {
        [Description("普通用户")]
        Common = 1,
        [Description("管理员")]
        Admin = 2
    }
}
