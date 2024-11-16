using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBlog.Models.Enums
{
    public enum EnumCommentStatuses
    {
        [Description("待审核")]
        Pending = 1,
        [Description("通过")]
        Approved = 2,
        [Description("拒绝")]
        Rejected = 3
    }
}
