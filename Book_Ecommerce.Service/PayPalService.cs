using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class PayPalService : IPayPalService
    {
        private readonly string _clientId;
        private readonly string _secretKey;
        private readonly IUrlHelper _urlHelper;
        private double _usdExchangeRate = 24500;

        public PayPalService(IConfiguration configuration, IUrlHelperFactory urlHelperFactory, IActionContextAccessor action)
        {
            _clientId = configuration["PaypalSettings:ClientId"];
            _secretKey = configuration["PaypalSettings:SecretKey"];
            _urlHelper = urlHelperFactory.GetUrlHelper(action.ActionContext ?? new ActionContext());
        }
        public async Task<string?> CreateUrlPayment(Domain.Entities.Order order ,string orderId, string customerId)
        {
            #region Create paypal Order
            var orderDetails = order.OrderDetails.ToList();
            var environment = new SandboxEnvironment(_clientId, _secretKey);
            var client = new PayPalHttpClient(environment);
            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };
            double amount = 0;
            foreach (var item in orderDetails)
            {
                itemList.Items.Add(new Item()
                {
                    Name = item.Product.ProductName,
                    Currency = "USD",
                    Price = Math.Round((double)item.Price / _usdExchangeRate, 2).ToString(),
                    Quantity = item.Quantity.ToString(),
                    Sku = "sku",
                    Tax = "0"
                });
                amount += Math.Round(((double)item.Price) / _usdExchangeRate, 2) * item.Quantity;
            }
            #endregion
            var paymentOrderId = DateTime.Now.Ticks;
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = new Amount()
                            {
                                Total = (amount + Math.Round(((double)order.TransportFee / _usdExchangeRate), 2)).ToString(),
                                Currency = "USD",
                                Details = new AmountDetails
                                {
                                    Tax = "0",
                                    Shipping = Math.Round(((double)order.TransportFee / _usdExchangeRate), 2).ToString(),
                                    Subtotal = amount.ToString()
                                }
                            },
                            ItemList = itemList,
                            Description = $"Invoice #{paymentOrderId}",
                            InvoiceNumber = paymentOrderId.ToString()
                        }
                    },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = _urlHelper.ActionLink("PaypalPaymentFail", "Checkout", new {orderId = orderId}),
                    ReturnUrl = _urlHelper.ActionLink("PaypalPaymentSuccess", "Checkout", new { customerId = customerId })
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };
            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);
            var response = await client.Execute(request);
            var statusCode = response.StatusCode;
            Payment result = response.Result<Payment>();

            var links = result.Links.GetEnumerator();
            string? paypalRedirectUrl = null;
            while (links.MoveNext())
            {
                LinkDescriptionObject lnk = links.Current;
                if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                {
                    //saving the payapalredirect URL to which user will be redirected for payment  
                    paypalRedirectUrl = lnk.Href;
                }
            }

            return paypalRedirectUrl;
        }
    }
}
