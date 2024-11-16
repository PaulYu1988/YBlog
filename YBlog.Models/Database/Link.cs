using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Link
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int Target { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }

    public int Sort { get; set; }
}
