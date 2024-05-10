using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book_Ecommerce.Domain.Validation;

namespace Book_Ecommerce.Domain.ViewModels.BrandViewModel
{
    public class InputBrand
    {
        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        public string BrandName { get; set; } = null!;
        public string? Description { get; set; }
        [ChkFileExtension(Extensions = "png,jpg,jpeg,gif", ErrorMessage = "File ảnh không đúng định dạng")]
        public IFormFile? FileImage { get; set; }
    }
}
