using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumArticleStatuses
    {
        [Description("草稿")]
        Draft = 1,
        [Description("发布")]
        Publish = 2
    }
}
