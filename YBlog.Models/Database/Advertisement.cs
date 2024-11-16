using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Advertisement
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? AdvertisementContent { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
