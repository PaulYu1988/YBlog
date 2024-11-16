using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class UserCredential
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserId { get; set; }

    public string? Token { get; set; }

    public DateTime? TokenExpirationTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
