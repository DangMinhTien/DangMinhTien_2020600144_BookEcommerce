using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("Banners")]
    public class Banner
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string BannerId { get; set; } = null!;
        [Column(TypeName = "nvarchar(250)")]
        public string Title { get; set; } = null!;
        [Column(TypeName = "nvarchar(500)")]
        public string Decription { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string Image { get; set; } = null!;
    }
}
