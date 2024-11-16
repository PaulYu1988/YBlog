using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumOperationReferenceTypes
    {
        [Description("用户")]
        User = 1,
        [Description("分类")]
        Category = 2,
        [Description("文章")]
        Article = 3,
        [Description("标签")]
        Tag = 4,
        [Description("评论")]
        Comment = 5,
        [Description("导航")]
        Nav = 6,
        [Description("友情链接")]
        Link = 7,
        [Description("广告代码")]
        Advertisement = 8,
        [Description("设置")]
        WebConfig = 9,
        [Description("Sitemap")]
        Sitemap = 10,
        [Description("文件")]
        File = 11
    }
}
