using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class UserInteraction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Type { get; set; }

    public int ArticleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
