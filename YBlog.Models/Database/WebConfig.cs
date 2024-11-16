using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class WebConfig
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ConfigKey { get; set; } = null!;

    public string? ConfigValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }

    public string? Description { get; set; }
}
