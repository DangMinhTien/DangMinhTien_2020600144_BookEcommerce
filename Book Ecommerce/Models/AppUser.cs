using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsActive { get; set; }
        [Column(TypeName = "char(36)")]
        public string? EmployeeId { get; set; }
        public Employee Employee { set; get; } = null!;
        [Column(TypeName = "char(36)")]
        public string? CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
