using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.EmployeeViewModel
{
    public class InputEmployee
    {
        [EmailAddress(ErrorMessage = "Địa chỉ email sai định dạng")]
        [Required(ErrorMessage = "Địa chỉ email không được để trống")]
        public string Email { get; set; } = null!;
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Mật khẩu phải có độ dài ít nhất 6 ký tự và phải có chữ hoa chữ thường, số và kí tự đặc biệt")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Giới tính không được để trống")]
        public bool Gender { get; set; }
        [RegularExpression(@"^(0[1-9])+([0-9]{8})\b$", ErrorMessage = "Số điện thoại sai định dạng")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = null!;
    }
}
