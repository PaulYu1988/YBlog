using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System.Text;
using YBlog.Models.Enums;

namespace YBlog.Areas.Common
{
    [Area("Common")]
    public class CaptchaController : Controller
    {
        public IActionResult Index()
        {
            var rand = new Random();
            int len = 6; //rand.Next(4, 6);
            char[] chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var myStr = new StringBuilder();
            for (int iCount = 0; iCount < len; iCount++)
            {
                myStr.Append(chars[rand.Next(chars.Length)]);
            }
            var captcha = myStr.ToString();
            Request.HttpContext.Session.SetString(EnumSessionNames.Captcha.ToString(), captcha);
            using (var bitmap = new SKBitmap(120, 40))
            {
                // 创建SKCanvas对象
                using (var canvas = new SKCanvas(bitmap))
                {
                    // 设置背景颜色
                    canvas.Clear(SKColors.White);
                    // 设置字体
                    using (var paint = new SKPaint())
                    {
                        paint.Color = SKColors.Black;
                        paint.IsAntialias = true;
                        paint.TextSize = 26;
                        // 将验证码绘制到画布上
                        canvas.DrawText(captcha, 14, 29, paint);
                    }
                    // 保存验证码图片
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = new MemoryStream())
                    {
                        data.SaveTo(stream);
                        return File(stream.ToArray(), "image/jpeg");
                    }
                }
            }
        }
    }
}
