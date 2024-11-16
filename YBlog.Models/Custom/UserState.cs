using YBlog.Models.Enums;

namespace YBlog.Models.Custom
{
    public class UserState
    {
        public bool IsLogin { get; set; }
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public EnumUserTypes Type { get; set; }
        public EnumUserStatuses Status { get; set; }
        public string Nickname { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Email { get; set; } = null!;
    }
}
