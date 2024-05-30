using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Authors")]
    public class Author
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string AuthorId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string AuthorCode { set; get; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string AuthorName { set; get; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string AuthorSlug { set; get; } = null!;
        [Column(TypeName = "nvarchar(500)")]
        public string? Information { set; get; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? UrlImage { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? FileImage { get; set; }
        public IEnumerable<AuthorProduct> AuthorProducts { get; set; } = null!;

    }
}
