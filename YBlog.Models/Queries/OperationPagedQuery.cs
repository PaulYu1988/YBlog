namespace YBlog.Models.Queries
{
    public class OperationPagedQuery : PagedQuery
    {
        public int? UserId { get; set; }
        public int? Type { get; set; }
        public int? ReferenceType { get; set; }
    }
}
