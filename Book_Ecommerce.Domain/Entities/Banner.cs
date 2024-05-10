using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Banners")]
    public class Banner
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string BannerId { get; set; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string BannerCode { set; get; } = null!;
        [Column(TypeName = "nvarchar(250)")]
        public string Title { get; set; } = null!;
        [Column(TypeName = "nvarchar(500)")]
        public string Content { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string ImageName { get; set; } = null!;
        public string UrlImage { get; set; } = null!;
    }
}
