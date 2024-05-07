using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string CategoryId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string CategoryCode { get; set; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string CategoryName { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string CategorySlug { get; set; } = null!;
        [Column(TypeName = "nvarchar(500)")] 
        public string? Decription { get; set; }
        public IEnumerable<CategoryProducts> CategoryProducts { get; set; } = null!;
    }
}
