using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumUserStatuses
    {
        [Description("正常")]
        Enabled = 1,
        [Description("禁用")]
        Disabled = 2
    }
}
