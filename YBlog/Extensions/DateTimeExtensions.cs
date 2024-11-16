namespace YBlog.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToViewString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.TotalSeconds < 60)
            {
                return (int)timeSpan.TotalSeconds + "秒前";
            }
            else if (timeSpan.TotalSeconds < 3600)
            {
                return (int)timeSpan.TotalMinutes + "分钟前";
            }
            else if (timeSpan.TotalSeconds < 3600 * 24)
            {
                return (int)timeSpan.TotalHours + "小时前";
            }
            else if (timeSpan.TotalDays < 6)
            {
                return (int)timeSpan.TotalDays + "天前";
            }
            else
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
        }

        public static string ToDateTimeString(this DateTime dateTime, string? format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }
    }
}
