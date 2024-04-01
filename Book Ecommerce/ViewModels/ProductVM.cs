using Book_Ecommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.ViewModels
{
    public class ProductVM
    {
        public string ProductId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string ProductSlug { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public double? PercentDiscount { get; set; }
        public string? Decription { get; set; }
        public int Quantity { get; set; }
        public Brand Brand { set; get; } = null!;
        public IEnumerable<Category> Categories { set; get; } = null!;
        public IEnumerable<Author> Authors { set; get; } = null!;
        public IEnumerable<Image> Images { get; set; } = null!;
        public IEnumerable<ProductVM> Products { get; set; } = null!;
        public IEnumerable<Comment> Comments { get; set; } = null!;
    }
}
