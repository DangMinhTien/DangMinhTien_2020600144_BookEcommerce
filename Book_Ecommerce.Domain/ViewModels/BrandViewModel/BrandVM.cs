﻿using Book_Ecommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.ViewModels.BrandViewModel
{
    public class BrandVM
    {
        public string BrandId { get; set; } = null!;
        public string BrandCode { set; get; } = null!;
        public string BrandName { set; get; } = null!;
        public string BrandSlug { set; get; } = null!;
        public bool IsActive { get; set; }
        public string? Decription { set; get; }
        public string Image { set; get; } = null!;
        public int SumProduct { set; get; }
        public IEnumerable<Product>? Products { get; set; }
    }
}