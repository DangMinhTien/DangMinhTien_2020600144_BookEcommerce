using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("Favourites")]
    public class Favourite
    {
        [Column(TypeName = "char(36)")] 
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        [Column(TypeName = "char(36)")] 
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
