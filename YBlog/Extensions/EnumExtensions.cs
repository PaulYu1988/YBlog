using System.ComponentModel;

namespace YBlog.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return string.Empty;
            var stringValue = value.ToString();
            var field = value.GetType().GetField(stringValue);
            if (field == null)
                return stringValue;
            var attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? stringValue : attribute.Description;
        }
    }
}
