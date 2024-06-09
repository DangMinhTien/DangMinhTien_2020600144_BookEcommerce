using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.ChatViewModel
{
    public class ChatVM
    {
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public string? LastMessage { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
