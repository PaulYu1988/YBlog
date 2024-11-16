using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class AdvertisementRequest
    {
        [Required]
        public int? Id { get; set; }
        public string? AdvertisementContent { get; set; }
        public bool IsEnabled { get; set; }
    }
}
