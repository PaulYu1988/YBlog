using System;
using System.Collections.Generic;

namespace YBlog.Models.Database
{
    public partial class Bot
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string IdentificationCode { get; set; } = null!;

        public int Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }
}
