using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Column(TypeName = "char(36)")]
        public string OrderId { get; set; } = null!;
        public Order Order { get; set; } = null!;
        [Column(TypeName = "char(36)")]
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
