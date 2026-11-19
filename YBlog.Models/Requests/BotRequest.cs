using System.ComponentModel.DataAnnotations;
using YBlog.Models.Enums;

namespace YBlog.Models.Requests
{
    public class BotRequest
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? IdentificationCode { get; set; }

        [Required]
        [EnumDataType(typeof(EnumBotTypes))]
        public int? Type { get; set; }
    }
}
