using YBlog.Models.Database;

namespace YBlog.Models.Custom
{
    public class OperationView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public int ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public object? Reference { get; set; }
        public User? User { get; set; }
    }
}
