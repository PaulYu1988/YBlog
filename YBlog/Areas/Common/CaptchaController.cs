using Microsoft.AspNetCore.Mvc;
using System.Text;
using YBlog.Models.Enums;
using ImageMagick;
using ImageMagick.Drawing;

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
            using (var image = new MagickImage(MagickColors.White, 120, 40))
            {
                var draw = new Drawables()
                .Font("Arial")
                .FontPointSize(26)
                .FillColor(MagickColors.Black)
                .TextAlignment(TextAlignment.Left)
                .Text(14, 29, captcha);
                image.Draw(draw);
                image.Format = MagickFormat.Jpeg;
                using (var stream = new MemoryStream())
                {
                    image.Write(stream);
                    return File(stream.ToArray(), "image/jpeg");
                }
            }
        }
    }
}
