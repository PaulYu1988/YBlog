namespace YBlog.Extensions
{
    public static class FormFileExtensions
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

        public static async Task<bool> IsValidImageAsync(this IFormFile? file)
        {
            if (file == null || file.Length <= 0)
            {
                return false;
            }
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension))
            {
                return false;
            }
            var header = new byte[12];
            using (var stream = file.OpenReadStream())
            {
                var read = await stream.ReadAsync(header.AsMemory(0, header.Length));
                return IsImageHeader(header, read);
            }
        }

        private static bool IsImageHeader(byte[] data, int length)
        {
            if (length < 2)
            {
                return false;
            }
            // JPEG
            if (length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
            {
                return true;
            }
            // PNG
            if (length >= 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47 && data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A)
            {
                return true;
            }
            // GIF87a / GIF89a
            if (length >= 6 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38)
            {
                return true;
            }
            // BMP
            if (data[0] == 0x42 && data[1] == 0x4D)
            {
                return true;
            }
            // WEBP
            if (length >= 12 && data[0] == 0x52 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x46 && data[8] == 0x57 && data[9] == 0x45 && data[10] == 0x42 && data[11] == 0x50)
            {
                return true;
            }
            return false;
        }
    }
}