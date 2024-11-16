namespace YBlog.Models.Custom
{
    public class ManageMenu
    {
        public string? Text { get; set; }
        public string? Icon { get; set; }
        public bool Active { get; set; }
        public List<ManageMenuItem> Items { get; set; } = new List<ManageMenuItem>();
    }
    public class ManageMenuItem
    {
        public string? Text { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public bool Active { get; set; }
    }
}
