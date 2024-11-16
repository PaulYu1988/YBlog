namespace YBlog.Models.Custom
{
    public class FileItem
    {
        public string? Name { get; set; }
        public string? RelativePath { get; set; }
        public string? Extension { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
