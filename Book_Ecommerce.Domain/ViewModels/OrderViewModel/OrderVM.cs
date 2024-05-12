using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.OrderViewModel
{
    public class OrderVM
    {
        public string OrderId { get; set; } = null!;
        public string OrderCode { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Status { get; set; }
        public decimal TransportFee { get; set; }
        public string PaymentType { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime DateDelivery { get; set; }
        public Customer Customer { get; set; } = null!;
        public int SumQuantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount
        {
            get
            {
                return Amount + TransportFee;
            }
        }
    }
}
