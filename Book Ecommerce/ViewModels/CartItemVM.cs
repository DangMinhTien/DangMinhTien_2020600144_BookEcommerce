namespace Book_Ecommerce.ViewModels
{
    public class CartItemVM
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string ProductSlug { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Amount
        {
            get
            {
                return Quantity * Price;
            }
        }
        public string Image { set; get; } = null!;
    }
}
