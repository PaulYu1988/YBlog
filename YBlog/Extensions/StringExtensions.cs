using System.Security.Cryptography;
using System.Text;

namespace YBlog.Extensions
{
    public static class StringExtensions
    {
        public static string ToMD5(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            // 将输入字符串转换为字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);

            // 创建MD5对象
            using (MD5 md5 = MD5.Create())
            {
                // 计算哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将哈希值转换为字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
