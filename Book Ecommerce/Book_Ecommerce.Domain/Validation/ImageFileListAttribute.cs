using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Domain.Validation
{
    public class ImageFileListAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (!IsImage(file))
                    {
                        return new ValidationResult(ErrorMessage ?? "All files must be images.");
                    }
                }
            }
            return ValidationResult.Success;
        }

        private bool IsImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedFormats = new[] { ".jpg", ".jpeg", ".png", ".gif" }; // Các định dạng ảnh cho phép

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return Array.IndexOf(allowedFormats, fileExtension) != -1;
        }
    }
}
