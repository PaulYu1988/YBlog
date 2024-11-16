using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class DailyStatistic
{
    public int Id { get; set; }

    public int Type { get; set; }

    public DateTime Date { get; set; }

    public int Count { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
