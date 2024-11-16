using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Log
{
    public int Id { get; set; }

    public int Type { get; set; }

    public string? LogContent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
