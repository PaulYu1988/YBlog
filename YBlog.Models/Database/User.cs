using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class User
{
    public int Id { get; set; }

    public int Type { get; set; }
    public int Status { get; set; }

    public string Nickname { get; set; } = null!;

    public string? Email { get; set; }

    public string? Avatar { get; set; }

    public string? Job { get; set; }

    public string? Mobile { get; set; }

    public string? Introduction { get; set; }

    public string? RegIp { get; set; }

    public DateTime LastLoginedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
