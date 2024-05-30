using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.CategoryViewModel
{
    public class InputCategory
    {
        [Required(ErrorMessage = "Tên thể loại không được để trống")]
        public string CategoryName { get; set; } = null!;
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Description { get; set; } = null!;
    }
}
