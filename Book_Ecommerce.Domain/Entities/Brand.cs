using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
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
        public string? Description { set; get; }
        [Column(TypeName = "varchar(250)")]
        public string ImageName { get; set; } = null!;
        public string UrlImage { get; set; } = null!;
        public IEnumerable<Product>? Products { get; set; }
    }
}
