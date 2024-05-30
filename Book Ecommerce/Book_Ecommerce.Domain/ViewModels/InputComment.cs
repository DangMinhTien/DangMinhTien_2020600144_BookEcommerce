using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels
{
    public class InputComment
    {
        [Required(ErrorMessage = "Bạn phải đánh giá số sao cho sản phẩm")]
        [Range(1, 5, ErrorMessage = "Số sao đánh giá phải trong khoảng {1} đến {2}")]
        public int? Vote {  get; set; }
        [Required(ErrorMessage = "Bạn phải bình luận cho sản phẩm")]
        public string Message { get; set; } = null!;
    }
}
