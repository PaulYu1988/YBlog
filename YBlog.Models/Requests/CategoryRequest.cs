using System.ComponentModel.DataAnnotations;

namespace YBlog.Models.Requests
{
    public class CategoryRequest
    {
        public int? Id { get; set; }
        [Required]
        public string? Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Sort { get; set; } = 50;
    }
}
