using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Tag
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
