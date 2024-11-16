using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int UserId { get; set; }

    public string ArticleContent { get; set; } = null!;

    public int CategoryId { get; set; }

    public bool IsTop { get; set; }

    public string? Thumbnail { get; set; }

    public int ReadCount { get; set; }

    public int CommentCount { get; set; }

    public bool IsDeleted { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
