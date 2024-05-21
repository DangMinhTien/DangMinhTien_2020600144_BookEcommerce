using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Domain.MySettings;
using Microsoft.AspNetCore.Authorization;
using Book_Ecommerce.Services;
using System.Text.Encodings.Web;
using PayPal.Core;
using PayPal.v1.Payments;
using BraintreeHttp;
using Book_Ecommerce.Data;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ExtendMethods;
using Book_Ecommerce.Domain.ViewModels.CheckoutViewModel;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;
using Microsoft.AspNetCore.Http;
using PayPal.v1.Orders;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signInManage;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManage;
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _config;
        private readonly ILogger<CheckoutController> _logger;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IProvinceService _provinceService;
        private readonly ICheckoutService _checkoutService;
        private readonly IPayPalService _payPalService;
        private IEnumerable<CartItemVM> _cart 
        {
            get
            {
                return HttpContext.Session.Get<List<CartItemVM>>(MyAppSetting.CART_KEY) ?? new List<CartItemVM>();
            }
        }
        public CheckoutController(AppDbContext context,
                SignInManager<AppUser> signInManager,
                UserManager<AppUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IVnPayService vnPayService,
                IConfiguration config,
                ILogger<CheckoutController> logger,
                ICartService cartService,
                ICustomerService customerService,
                IProductService productService,
                IProvinceService provinceService,
                ICheckoutService checkoutService,
                IPayPalService payPalService)
        {
            _context = context;
            _signInManage = signInManager;
            _userManager = userManager;
            _roleManage = roleManager;
            _vnPayService = vnPayService;
            _config = config;
            _logger = logger;
            _cartService = cartService;
            _customerService = customerService;
            _productService = productService;
            _provinceService = provinceService;
            _checkoutService = checkoutService;
            _payPalService = payPalService;
        }
        [HttpGet("/dat-hang")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể mở được phần đặt hàng do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể mở được phần đặt hàng do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var cartItemVMs = await _cartService.GetByCustomerToViewAsync(customer.CustomerId);
                if(cartItemVMs ==  null || cartItemVMs.Count() == 0)
                {
                    TempData["infor"] = "Không thể mở phần đặt hàng do giỏ hàng của bạn chưa có sản phẩm nào";
                    return RedirectToAction("Index", "Products");
                }
                foreach (var item in cartItemVMs)
                {
                    var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == item.ProductId);
                    if (product == null)
                    {
                        TempData["error"] = "Không thể đặt hàng do không tìm thấy sản phẩm";
                        return RedirectToAction("Index", "Cart");
                    }
                    if (product.Quantity < item.Quantity)
                    {
                        TempData["error"] = $"Không thể đặt hàng do số lượng của sản phẩm {product.ProductName} " +
                            $"chỉ là {product.Quantity} nên không đủ";
                        return RedirectToAction("Index", "Cart");
                    }
                }
                HttpContext.Session.Set<List<CartItemVM>>(MyAppSetting.CART_KEY, (List<CartItemVM>)cartItemVMs);
                var checkoutVM = new CheckoutVM
                {
                    FullName = customer.FullName,
                    PhoneNumber = user.PhoneNumber,
                    CartItemVMs = cartItemVMs,
                };
                return View(checkoutVM);
            }
            catch
            {
                TempData["error"] = "Không thể mở được phần đặt hàng";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("/trang-thanh-toan")]
        public async Task<IActionResult> ViewPay(CheckoutVM checkout)
        {
            try
            {
                var cartItemVMs = _cart;
                if(cartItemVMs.Count() == 0)
                {
                    TempData["error"] = "Không thể đặt hàng do giỏ hàng của bạn trống";
                    return RedirectToAction("Index", "Home");
                }
                if(cartItemVMs.Any(c => _productService.GetSingleByCondition(p => p.ProductId == c.ProductId) == null))
                {
                    HttpContext.Session.Set<List<CartItemVM>>(MyAppSetting.CART_KEY, new List<CartItemVM>());
                    TempData["error"] = "Không thể đặt hàng do không thể tìm thấy sản phẩm bạn đặt";
                    return RedirectToAction("Index", "Home");
                }
                checkout.CartItemVMs = cartItemVMs;
                if(ModelState.IsValid)
                {
                    var provinceName = (await _provinceService.GetSingleProvinceByConditionAsync(p => p.Code == checkout.Province))?.FullName;
                    var districtName = (await _provinceService.GetSingleDistrictByConditionAsync(d => d.Code == checkout.District))?.FullName;
                    var wardName = (await _provinceService.GetSingleWardByConditionAsync(w => w.Code == checkout.Ward))?.FullName;
                    var payVM = new PayVM
                    {
                        FullName = checkout.FullName,
                        PhoneNumber = checkout.PhoneNumber,
                        Note = checkout.Note,
                        Address = $"{checkout.AddressDetail} - {wardName} - {districtName} - {provinceName}",
                        TransportFee = $"{checkout.AddressDetail}{wardName}{districtName}{provinceName}".Length * 1000,
                        CartItemVMs = cartItemVMs
                    };
                    return View("Pay", payVM);
                }
                return View(checkout);
            }
            catch
            {
                TempData["error"] = " Đặt hàng thất bại do lỗi hệ thống";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("/thanh-toan")]
        public async Task<IActionResult> Pay(PayVM payVM, string paymentType = MyPayment.COD)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể đặt hàng do không tìm thấy tài khoản đăng nhập";
                    return View("Failure");
                }
                var customer = await _customerService.Table().Include(c => c.CartItems)
                                                        .FirstOrDefaultAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể đặt hàng do không tìm thấy khách hàng đăng nhập";
                    return View("Failure");
                }
                var cartItemVMs = _cart;
                if (cartItemVMs.Count() == 0)
                {
                    TempData["error"] = "Không thể đặt hàng do giỏ hàng của bạn trống";
                    return View("Failure");
                }
                if (cartItemVMs.Any(c => _productService.GetSingleByCondition(p => p.ProductId == c.ProductId) == null))
                {
                    HttpContext.Session.Set<List<CartItemVM>>(MyAppSetting.CART_KEY, new List<CartItemVM>());
                    TempData["error"] = "Không thể đặt hàng do không thể tìm thấy sản phẩm bạn đặt";
                    return View("Failure");
                }
                if (ModelState.IsValid)
                {
                    int statusOrder = (int)StatusOrder.DaDatHang;
                    if (paymentType != MyPayment.COD)
                        statusOrder = (int)StatusOrder.DaThanhToan;
                    var maxNumber = _checkoutService.OrderTable().Count() == 0 ? 
                        1000 : _checkoutService.OrderTable().Max(o => o.CodeNumber) + 1;
                    var order = new Book_Ecommerce.Domain.Entities.Order
                    {
                        OrderId = Guid.NewGuid().ToString() ,
                        CodeNumber = maxNumber,
                        OrderCode = $"DH{maxNumber}{DateTime.Now.ToString("yyyyMMddHHmmss")}",
                        Status = statusOrder,
                        DateCreated = DateTime.Now,
                        DateDelivery = DateTime.Now.AddDays(3),
                        FullName = payVM.FullName,
                        Address = payVM.Address,
                        TransportFee = payVM.TransportFee,
                        PhoneNumber = payVM.PhoneNumber,
                        Note = payVM.Note,
                        PaymentType = paymentType,
                        CustomerId = customer.CustomerId
                    };
                    var orderDetails = new List<OrderDetail>();
                    foreach (var item in cartItemVMs)
                    {
                        var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == item.ProductId);
                        if (product == null)
                        {
                            TempData["error"] = "Không thể đặt hàng do không tìm thấy sản phẩm";
                            return View("Failure");
                        }
                        if(product.Quantity < item.Quantity)
                        {
                            TempData["error"] = $"Không thể đặt hàng do số lượng của sản phẩm {product.ProductName} " +
                                $"chỉ là {product.Quantity} nên không đủ";
                            return View("Failure");
                        }
                        orderDetails.Add(new OrderDetail
                        {
                            ProductId = item.ProductId,
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                    }
                    await _context.Database.BeginTransactionAsync();
                    await _checkoutService.AddOrderAsync(order, orderDetails);
                    if (paymentType == MyPayment.COD)
                    {
                        await _cartService.RemoveRangeAsync(customer.CartItems);
                    }
                    await _context.Database.CommitTransactionAsync();
                    if(paymentType == MyPayment.VnPay)
                    {
                        var vnpayModel = new VnPaymentRequestModel
                        {
                            OrderId = order.OrderCode,
                            Amount = orderDetails.Sum(od => od.Quantity * (double)od.Price) + (double)order.TransportFee,
                            CreatedDate = DateTime.Now,
                            Decription = $"Thanh toán đơn hàng {order.OrderCode} từ Minh Tiến BookStore",
                            FullName = customer.FullName
                        };
                        var paymentBackUrl = Url.ActionLink(nameof(VnPaymentCallBack), values: new
                        {
                            orderId = order.OrderId,
                            customerId = customer.CustomerId,
                        }) ?? _config["VnPay:PaymentBackUrl"] + $"?orderId={order.OrderId}&customerId={customer.CustomerId}";
                        return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnpayModel, paymentBackUrl));
                    }
                    if(paymentType == MyPayment.PayPal)
                    {
                        return RedirectToAction(nameof(PaypalPayment), new 
                        { 
                            orderId = order.OrderId, 
                            customerId = customer.CustomerId
                        });
                    }
                    TempData["success"] = "Đặt hàng thành công";
                    return View("Success");
                }
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync();
            }
            TempData["error"] = " Đặt hàng thất bại do lỗi hệ thống";
            return View("Failure");
        }
        [HttpGet("/ket-qua-thanh-toan-vppay")]
        public async Task<IActionResult> VnPaymentCallBack(string orderId, string customerId)
        {
            try
            {
                var response = _vnPayService.PaymentExcute(Request.Query);
                if (response == null || response.VnPayResponseCode != "00")
                {
                    var order = await _checkoutService.GetSingleOrderByConditionAsync(o => o.OrderId == orderId);
                    if(order != null)
                    {
                        await _checkoutService.RemoveOrderAsync(order);
                    }
                    TempData["error"] = "Thanh toán bằng VnPay thất bại";
                    return View("Failure");
                }
                var cartItems = await _cartService.GetDataAsync(c => c.CustomerId == customerId);
                if(cartItems.Count() > 0)
                {
                    await _cartService.RemoveRangeAsync(cartItems);
                }
                TempData["success"] = "Thanh toán bằng VnPay thành công";
                return View("Success");

            }
            catch
            {
                TempData["error"] = "Thanh toán bằng VnPay thất bại";
                return View("Failure");
            }
        }
        [HttpGet("/thanh-toan-paypal")]
        public async System.Threading.Tasks.Task<IActionResult> PaypalPayment(string orderId, string customerId)
        {
            try
            {
                var order = await _checkoutService.OrderTable()
                                        .Include(o => o.OrderDetails)
                                        .ThenInclude(od => od.Product)
                                        .FirstOrDefaultAsync(c => c.OrderId == orderId);
                if (order == null)
                {
                    TempData["error"] = "Đặt hàng thất bại do thanh toán Paypal không thành công";
                    return View("Failure");
                }
                if (order.OrderDetails.Any(od => _productService.GetSingleByCondition(p => p.ProductId == od.ProductId) == null))
                {
                    await _checkoutService.RemoveOrderAsync(order);
                    TempData["error"] = "Đặt hàng thất bại do không tìm thấy sản phẩm";
                    return View("Failure");
                }
                var paypalRedirectUrl = await _payPalService.CreateUrlPayment(order, orderId, customerId);
                if (string.IsNullOrEmpty(paypalRedirectUrl))
                {
                    return RedirectToAction("PaypalPaymentFail", "Checkout", new { orderId = orderId });
                }
                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                //Process when Checkout with Paypal fails
                TempData["error"] = "Đặt hàng thất bại do thanh toán Paypal không thành công" + httpException.Message;
                return RedirectToAction(nameof(PaypalPaymentFail), new {orderId = orderId});
            }
        }
        [HttpGet("/thanh-toan-paypal-that-bai")]
        public async Task<IActionResult> PaypalPaymentFail(string orderId)
        {
            try
            {
                var order = await _checkoutService.GetSingleOrderByConditionAsync(o => o.OrderId == orderId);
                if(order != null)
                {
                    await _checkoutService.RemoveOrderAsync(order);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            TempData["error"] = "Đặt hàng thất bại do thanh toán Paypal không thành công";
            return View("Failure");
        }
        [HttpGet("/thanh-toan-paypal-thanh-cong")]
        public async Task<IActionResult> PayPalPaymentSuccess(string customerId)
        {
            try
            {
                var cartItems = await _cartService.GetDataAsync(c => c.CustomerId == customerId);
                if(cartItems.Count() > 0)
                {
                    await _cartService.RemoveRangeAsync(cartItems);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            TempData["success"] = "Thanh toán Paypal thành công";
            return View("Success");
        }
    }
}
