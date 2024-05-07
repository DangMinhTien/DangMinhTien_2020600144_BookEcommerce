namespace Book_Ecommerce.Domain.Models
{
    public class VnPaymentRequestModel
    {
        public string OrderId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Decription { get; set; } = null!;
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
