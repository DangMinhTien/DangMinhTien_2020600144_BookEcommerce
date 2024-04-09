using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string OrderId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string OrderCode { get; set; } = null!;
        public long CodeNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string FullName { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "nvarchar(250)")]
        public string Address { get; set; } = null!;
        public int Status { get; set; }
        public decimal TransportFee { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string? Note { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateDelivery { get; set; }
        [Column(TypeName = "char(36)")]
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = null!;
    }
}
