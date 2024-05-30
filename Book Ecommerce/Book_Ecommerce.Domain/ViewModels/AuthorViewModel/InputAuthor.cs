using Book_Ecommerce.Domain.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.ViewModels.AuthorViewModel
{
    public class InputAuthor
    {
        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        public string AuthorName { get; set; } = null!;
        public string? Information { get; set; }
        [ChkFileExtension(Extensions = "png,jpg,jpeg,gif", ErrorMessage = "File ảnh không đúng định dạng")]
        public IFormFile? FileImage { get; set; }
    }
}
