using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.AccountViewModel
{
    public class InputChangePassword
    {
        [Required(ErrorMessage = "Bạn phải nhập mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = null!;
        [Required(ErrorMessage = "Bạn phải nhập mật khẩu mới")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Mật khẩu mới phải có độ dài ít nhất 6 ký tự và phải có chữ hoa chữ thường, số và kí tự đặc biệt")]
        public string NewPassword { get; set; } = null!;
    }
}
