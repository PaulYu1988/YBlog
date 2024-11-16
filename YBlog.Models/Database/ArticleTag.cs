using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class ArticleTag
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int TagId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }

    public int Sort { get; set; }
}
