using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Comment
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public string CommentContent { get; set; } = null!;

    public int? TargetCommentId { get; set; }

    public string? Ip { get; set; }

    public int? UserId { get; set; }

    public int Status { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
