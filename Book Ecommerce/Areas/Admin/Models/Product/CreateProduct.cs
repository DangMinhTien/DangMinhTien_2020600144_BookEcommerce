﻿using System.ComponentModel.DataAnnotations;

namespace Book_Ecommerce.Areas.Admin.Models.Product
{
    public class CreateProduct
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string ProductName { get; set; } = null!;
        public string? ProductSlug { get; set; }
        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hoặc bằng {1}")]
        public double? Price {  get; set; }
        [Range(0.5, 100, ErrorMessage = "Phần trăm giảm giá phải lớn hơn bằng {1} và nhỏ hơn bằng {2}")]
        public double? PercentDiscount { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Decreption { get; set; } = null!;
        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn bằng {1}")]
        public int? Quantity { get; set; }
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Thương hiệu không được để trống")]
        public string BrandId { get; set; } = null!;
        [Required(ErrorMessage = "Bạn phải chọn thể loại cho sản phẩm")]
        public string[] CategoryIds { get; set; } = null!;
        public string[]? AuthorIds { get; set; }
        [Required(ErrorMessage = "Bạn phải chọn ảnh cho sản phẩm")]
        public List<IFormFile> ImageFiles { get; set; } = null!;
    }
}
