using System;
using System.Collections.Generic;

namespace YBlog.Models.Database;

public partial class Operation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Type { get; set; }

    public int ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
}
