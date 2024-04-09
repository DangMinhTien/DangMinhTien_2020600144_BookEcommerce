using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.ViewModels
{
    public class LoginVM
    {
        [EmailAddress(ErrorMessage = "Địa chỉ email sai định dạng")]
        [Required(ErrorMessage = "Địa chỉ email không được để trống")]
        public string Email { get; set; } = null!;
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Mật khẩu phải có độ dài ít nhất 6 ký tự và phải có chữ hoa chữ thường, số và kí tự đặc biệt")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;
    }
}
