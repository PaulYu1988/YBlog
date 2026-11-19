using System.ComponentModel;

namespace YBlog.Models.Enums
{
    public enum EnumBotTypes
    {
        /// <summary>
        /// 搜索引擎：Google、Bing、百度、Yandex 等
        /// </summary>
        [Description("搜索引擎")]
        SearchEngine = 1,

        /// <summary>
        /// AI 训练及数据采集：GPTBot、ClaudeBot、Bytespider、CCBot 等
        /// </summary>
        [Description("AI 训练及数据采集")]
        AITraining = 2,

        /// <summary>
        /// AI 搜索及智能代理：OAI-SearchBot、ChatGPT-User、PerplexityBot 等
        /// </summary>
        [Description("AI 搜索及智能代理")]
        AISearch = 3,

        /// <summary>
        /// 社交媒体：Facebook、Twitter、LinkedIn、微信等
        /// </summary>
        [Description("社交媒体")]
        SocialMedia = 4,

        /// <summary>
        /// SEO 工具：Ahrefs、Semrush、Moz 等
        /// </summary>
        [Description("SEO 工具")]
        SeoTools = 5,

        /// <summary>
        /// 网站监控及检测工具：UptimeRobot、Pingdom、RSS 等
        /// </summary>
        [Description("网站监控及检测工具")]
        WebsiteTools = 6,

        /// <summary>
        /// 安全扫描工具：Nessus、Nmap、sqlmap、Shodan 等
        /// </summary>
        [Description("安全扫描工具")]
        SecurityScanner = 7,

        /// <summary>
        /// 自动化客户端工具：curl、Python、Playwright、AI Agent 等
        /// </summary>
        [Description("自动化客户端工具")]
        AutomationTools = 8
    }
}
