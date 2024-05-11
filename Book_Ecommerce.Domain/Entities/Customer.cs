using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string CustomerId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string CustomerCode { get; set; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string FullName { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string? Address { get; set; }
        public AppUser User { get; set; } = null!;
        public IEnumerable<Order> Orders { get; set; } = null!;
        public IEnumerable<Comment> Comments { get; set; } = null!;
        public IEnumerable<FavouriteProduct> FavouriteProducts { get; set; } = null!;
        public IEnumerable<CartItem> CartItems { get; set; } = null!;
    }
}
