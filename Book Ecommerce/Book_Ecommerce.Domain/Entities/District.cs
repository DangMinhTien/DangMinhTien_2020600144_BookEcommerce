using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Districts")]
    public class District
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
        [Column(TypeName = "nvarchar(20)")]
        public string ProvinceCode { get; set; } = null!;
        public Province Province { get; set; } = null!;
        public IEnumerable<Ward> Wards { get; set; } = null!;
    }
}
