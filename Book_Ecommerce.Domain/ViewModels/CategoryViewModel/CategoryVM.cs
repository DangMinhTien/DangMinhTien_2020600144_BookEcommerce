using Book_Ecommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.Domain.ViewModels.CategoryViewModel
{
    public class CategoryVM
    {
        public string CategoryId { get; set; } = null!;
        public string CategoryCode { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string CategorySlug { get; set; } = null!;
        public string? Decription { get; set; }
        public int SumProduct { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Product> Products { get; set; } = null!;
    }
}
