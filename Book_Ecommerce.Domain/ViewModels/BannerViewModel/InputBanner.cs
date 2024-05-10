using Book_Ecommerce.Domain.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.BannerViewModel
{
    public class InputBanner
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = null!;
        [ChkFileExtension(Extensions = "png,jpg,jpeg,gif", ErrorMessage = "File ảnh không đúng định dạng")]
        public IFormFile? FileImage { get; set; }
    }
}
