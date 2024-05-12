using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.AccountViewModel
{
    public class InputAccount
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Giới tính không được để trống")]
        public bool Gender { get; set; }
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = null!;
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[1-9])+([0-9]{8})\b$", ErrorMessage = "Số điện thoại sai định dạng")]
        public string PhoneNumber { get; set; } = null!;
    }
}
