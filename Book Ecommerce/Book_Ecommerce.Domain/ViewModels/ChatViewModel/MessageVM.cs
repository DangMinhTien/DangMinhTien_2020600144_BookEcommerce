using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.ChatViewModel
{
    public class MessageVM
    {
        public string MessageId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SendDate { get; set; }
        public string SendBy { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
