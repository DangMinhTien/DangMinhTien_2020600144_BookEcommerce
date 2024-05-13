using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string CommentId { get; set; } = null!;
        public int Vote { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Message { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
        [Column(TypeName = "char(36)")]
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        [Column(TypeName = "char(36)")]
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;

    }
}
