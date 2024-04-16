using Book_Ecommerce.ViewModels;

namespace Book_Ecommerce.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string paymentBackUrl);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
