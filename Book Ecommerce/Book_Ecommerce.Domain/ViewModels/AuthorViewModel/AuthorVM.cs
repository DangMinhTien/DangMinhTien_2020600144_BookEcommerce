using Book_Ecommerce.Domain.Entities;

namespace Book_Ecommerce.Domain.ViewModels.AuthorViewModel
{
    public class AuthorVM
    {
        public string AuthorId { get; set; } = null!;
        public string AuthorCode { set; get; } = null!;
        public string AuthorName { set; get; } = null!;
        public string AuthorSlug { set; get; } = null!;
        public string? Information { set; get; }
        public string? UrlImage { set; get; }
        public string? FileImage { set; get; }
        public int SumProduct { set; get; }
        public bool IsActive { get; set; }
        public IEnumerable<Product> Products { get; set; } = null!;
    }
}
