using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.Domain.ViewModels.AccountViewModel
{
    public class LoginVM
    {
        [EmailAddress(ErrorMessage = "Địa chỉ email sai định dạng")]
        [Required(ErrorMessage = "Địa chỉ email không được để trống")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;
    }
}
