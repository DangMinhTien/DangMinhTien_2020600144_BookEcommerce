using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("CategoryProducts")]
    public class CategoryProducts
    {
        [Column(TypeName = "char(36)")]
        public string CategoryId { get; set; } = null!;
        public Category Category { set; get; } = null!;
        [Column(TypeName = "char(36)")]
        public string ProductId { get; set; } = null!;
        public Product Product { set; get; } = null!;
    }
}
