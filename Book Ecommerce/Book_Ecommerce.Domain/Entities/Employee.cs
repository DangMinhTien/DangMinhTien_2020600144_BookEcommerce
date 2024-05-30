using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [Column(TypeName = "char(36)")]
        public string EmployeeId { get; set; } = null!;
        [Column(TypeName = "varchar(250)")]
        public string EmployeeCode { get; set; } = null!;
        public long CodeNumber { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string FullName { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Address { set; get; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
