using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.Domain.ViewModels.CheckoutViewModel
{
    public class PayVM
    {
        public string FullName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal TransportFee { get; set; }
        public IEnumerable<CartItemVM>? CartItemVMs { get; set; }
        public string? Note { get; set; }
    }
}
