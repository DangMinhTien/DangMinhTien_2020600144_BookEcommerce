using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("AuthorProducts")]
    public class AuthorProduct
    {
        [Column(TypeName = "char(36)")]
        public string AuthorId { get; set; } = null!;
        public Author Author { get; set; } = null!;
        [Column(TypeName = "char(36)")]
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
