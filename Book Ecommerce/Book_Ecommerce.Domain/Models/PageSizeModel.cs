namespace Book_Ecommerce.Domain.Models
{
    public class PageSizeModel
    {
        public int Size { get; set; }
        public bool IsActive { get; set; }
        public string Url { get; set; } = null!;
    }
}
