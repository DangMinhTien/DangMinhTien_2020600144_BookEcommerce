using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book_Ecommerce.Domain.Entities;

namespace Book_Ecommerce.Domain.ViewModels.CustomerViewModel
{
    public class CustomerVM
    {
        public string CustomerId { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? Address { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
