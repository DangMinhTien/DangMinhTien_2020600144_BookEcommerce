using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string ProductId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string ProductCode { get; set; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string ProductName { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string ProductSlug { get; set; } = null!;
        public decimal Price { get; set; }
        public double? PercentDiscount { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "char(36)")]
        public string BrandId { get; set; } = null!;
        public Brand Brand { set; get; } = null!;
        public IEnumerable<CategoryProducts> CategoryProducts { get; set; } = null!;
        public IEnumerable<Image> Images { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = null!;
        public IEnumerable<FavouriteProduct> FavouriteProducts { get; set; } = null!;
        public IEnumerable<Comment> Comments { get; set; } = null!;
        public IEnumerable<AuthorProduct> AuthorProducts { get; set; } = null!;
        public IEnumerable<CartItem> CartItems { get; set; } = null!;
    }
}
