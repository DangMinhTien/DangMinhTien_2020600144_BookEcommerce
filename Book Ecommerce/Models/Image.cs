using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    [Table("Images")]
    public class Image
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string ImageId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string ImageName { get; set; } = null!;
        [Column(TypeName = "char(36)")] 
        public string ProductId { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
