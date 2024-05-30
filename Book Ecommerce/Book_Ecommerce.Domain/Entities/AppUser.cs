using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "char(36)")]
        public string? EmployeeId { get; set; }
        public Employee? Employee { set; get; }
        [Column(TypeName = "char(36)")]
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
