using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class ReadCountRequest
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string? Token { get; set; }
    }
}
