using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.Domain.ViewModels.CheckoutViewModel
{
    public class CheckoutVM
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Số nhà - Tên đường không được để trống")]
        public string AddressDetail { get; set; } = null!;
        [RegularExpression(@"^(0[1-9])+([0-9]{8})\b$", ErrorMessage = "Số điện thoại sai định dạng")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "Tỉnh/thành không được để trống")]
        public string Province { get; set; } = null!;
        [Required(ErrorMessage = "Quận/huyện không được để trống")]
        public string District { get; set; } = null!;
        [Required(ErrorMessage = "Phường/xã không được để trống")]
        public string Ward { get; set; } = null!;
        public IEnumerable<CartItemVM>? CartItemVMs { get; set; }
        public string? Note { get; set; }
    }
}
