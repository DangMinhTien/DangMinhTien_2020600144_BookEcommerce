using System.ComponentModel.DataAnnotations.Schema;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.Models
{
    [Table("Provinces")]
    public class Province
    {
        [Key]
        [Column(TypeName = "nvarchar(20)")]
        public string Code { get; set; } = null!;
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "nvarchar(255)")]
        public string? NameEn { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string FullName { get; set; } = null!;
        [Column(TypeName = "nvarchar(255)")]
        public string? FullNameEn { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? CodeName { get; set; }
        public IEnumerable<District> Districts { get; set; } = null!;
    }
}
