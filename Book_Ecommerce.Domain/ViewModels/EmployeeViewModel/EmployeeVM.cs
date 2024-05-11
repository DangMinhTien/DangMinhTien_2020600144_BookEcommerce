using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.EmployeeViewModel
{
    public class EmployeeVM
    {
        public string EmployeeId { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string Address { set; get; } = null!;
        public AppUser User { get; set; } = null!;
        public List<string> RoleNames { get; set; } = null!;
    }
}
