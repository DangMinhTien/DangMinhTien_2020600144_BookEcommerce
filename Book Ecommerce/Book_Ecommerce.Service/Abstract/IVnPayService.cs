using Book_Ecommerce.Domain.Library;
using Book_Ecommerce.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Book_Ecommerce.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string paymentBackUrl);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
