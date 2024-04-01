using Book_Ecommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Ecommerce.ViewModels
{
    public class AuthorVM
    {
        public string AuthorId { get; set; } = null!;
        public string AuthorCode { set; get; } = null!;
        public string AuthorName { set; get; } = null!;
        public string AuthorSlug { set; get; } = null!;
        public string? Information { set; get; }
        public int SumProduct {  set; get; }
        public bool IsActive { get; set; }
        public IEnumerable<Product> Products { get; set; } = null!;
    }
}
