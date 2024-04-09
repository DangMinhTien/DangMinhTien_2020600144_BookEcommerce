using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("Brands")]
    public class Brand
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string BrandId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string BrandCode { set; get; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string BrandName { set; get; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string BrandSlug { set; get; } = null!;
        [Column(TypeName = "nvarchar(500)")]
        public string? Decription { set; get; }
        [Column(TypeName = "varchar(250)")]
        public string Image { set; get; } = null!;
        public IEnumerable<Product>? Products { get; set; }
    }
}
