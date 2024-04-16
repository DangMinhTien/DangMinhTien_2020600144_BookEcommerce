using Book_Ecommerce.Helpers;
using Book_Ecommerce.Models;
using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Book_Ecommerce.Services;
using System.Text.Encodings.Web;
using PayPal.Core;
using PayPal.v1.Payments;
using BraintreeHttp;

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
        private readonly string _clientId;
        private readonly string _secretKey;
        private readonly ILogger<CheckoutController> _logger;
        private double _usdExchangeRate = 24500;
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
                ILogger<CheckoutController> logger)
        {
            _context = context;
            _signInManage = signInManager;
            _userManager = userManager;
            _roleManage = roleManager;
            _vnPayService = vnPayService;
            _config = config;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
            _logger = logger;
        }
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
                var customer = await _context.Customers.Include(c => c.CartItems)
                                                        .ThenInclude(c => c.Product)
                                                        .ThenInclude(p => p.Images)
                                                        .FirstOrDefaultAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể mở được phần đặt hàng do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                if(customer.CartItems.Count() == 0)
                {
                    TempData["infor"] = "Không thể mở phần đặt hàng do giỏ hàng của bạn chưa có sản phẩm nào";
                    return RedirectToAction("Index", "Products");
                }
                var cartItemVM = customer.CartItems.Select(c => new CartItemVM
                {
                    ProductId = c.ProductId,
                    ProductName = c.Product.ProductName,
                    ProductSlug = c.Product.ProductSlug,
                    Price = (c.Product.PercentDiscount == null || c.Product.PercentDiscount == 0) ? c.Product.Price 
                                    : c.Product.Price - (c.Product.Price * (decimal)c.Product.PercentDiscount/100),
                    Image = c.Product.Images.FirstOrDefault()?.ImageName ?? "",
                    ProductCode = c.Product.ProductCode,
                    Quantity = c.Quantity
                }).ToList();
                HttpContext.Session.Set<List<CartItemVM>>(MyAppSetting.CART_KEY, cartItemVM);
                var checkoutVM = new CheckoutVM
                {
                    FullName = customer.FullName,
                    PhoneNumber = user.PhoneNumber,
                    CartItemVMs = cartItemVM,
                };
                return View(checkoutVM);
            }
            catch
            {
                TempData["error"] = "Không thể mở được phần đặt hàng";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(CheckoutVM checkout)
        {
            try
            {
                var cartItemVMs = _cart;
                if(cartItemVMs.Count() == 0)
                {
                    TempData["error"] = "Không thể đặt hàng do giỏ hàng của bạn trống";
                    return RedirectToAction("Index", "Home");
                }
                if(cartItemVMs.Any(c => _context.Products.FirstOrDefault(p => p.ProductId == c.ProductId) == null))
                {
                    HttpContext.Session.Set<List<CartItemVM>>(MyAppSetting.CART_KEY, new List<CartItemVM>());
                    TempData["error"] = "Không thể đặt hàng do không thể tìm thấy sản phẩm bạn đặt";
                    return RedirectToAction("Index", "Home");
                }
                checkout.CartItemVMs = cartItemVMs;
                if(ModelState.IsValid)
                {
                    var provinceName = (await _context.Provinces.FirstOrDefaultAsync(p => p.Code == checkout.Province))?.FullName ?? "";
                    var districtName = (await _context.Districts.FirstOrDefaultAsync(d => d.Code == checkout.District))?.FullName ?? "";
                    var wardName = (await _context.Wards.FirstOrDefaultAsync(w => w.Code == checkout.Ward))?.FullName ?? "";
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
        [HttpPost]
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
                var customer = await _context.Customers.Include(c => c.CartItems)
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
                if (cartItemVMs.Any(c => _context.Products.FirstOrDefault(p => p.ProductId == c.ProductId) == null))
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
                    var maxNumber = _context.Orders.Count() == 0 ? 1000 : _context.Orders.Max(o => o.CodeNumber) + 1;
                    var order = new Book_Ecommerce.Models.Order
                    {
                        OrderId = Guid.NewGuid().ToString(),
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
                    var OrderDetails = new List<OrderDetail>();
                    foreach (var item in cartItemVMs)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
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
                        OrderDetails.Add(new OrderDetail
                        {
                            ProductId = item.ProductId,
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                    }
                    await _context.Database.BeginTransactionAsync();
                    _context.Orders.Add(order);
                    await _context.OrderDetails.AddRangeAsync(OrderDetails);
                    if(paymentType == MyPayment.COD)
                    {
                        _context.CartItems.RemoveRange(customer.CartItems);
                    }
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    if(paymentType == MyPayment.VnPay)
                    {
                        var vnpayModel = new VnPaymentRequestModel
                        {
                            OrderId = order.OrderCode,
                            Amount = OrderDetails.Sum(od => od.Quantity * (double)od.Price) + (double)order.TransportFee,
                            CreatedDate = DateTime.Now,
                            Decription = $"Thanh toán đơn hàng {order.OrderCode}",
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
        public async Task<IActionResult> VnPaymentCallBack(string orderId, string customerId)
        {
            try
            {
                var response = _vnPayService.PaymentExcute(Request.Query);
                if (response == null || response.VnPayResponseCode != "00")
                {
                    var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if(order != null)
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                    }
                    TempData["error"] = "Thanh toán bằng VnPay thất bại";
                    return View("Failure");
                }
                var cartItems = await _context.CartItems.Where(c => c.CustomerId == customerId).ToListAsync();
                if(cartItems.Count > 0)
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> PaypalPayment(string orderId, string customerId)
        {
            try
            {
                var environment = new SandboxEnvironment(_clientId, _secretKey);
                var client = new PayPalHttpClient(environment);
                var order = await _context.Orders
                                            .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Product)
                                            .FirstOrDefaultAsync(c => c.OrderId == orderId);
                if (order == null)
                {
                    TempData["error"] = "Đặt hàng thất bại do thanh toán Paypal không thành công";
                    return View("Failure");
                }
                if (order.OrderDetails.Any(od => _context.Products.FirstOrDefault(p => p.ProductId == od.ProductId) == null))
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    TempData["error"] = "Đặt hàng thất bại do không tìm thấy sản phẩm";
                    return View("Failure");
                }
                var orderDetails = order.OrderDetails.ToList();
                #region Create paypal Order
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
                    amount += Math.Round(((double)item.Price)/ _usdExchangeRate, 2) * item.Quantity;
                }
                #endregion
                var hostName = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
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
                        CancelUrl = $"{hostName}/Checkout/PaypalPaymentFail?orderId={orderId}&customerId={customerId}",
                        ReturnUrl = $"{hostName}/Checkout/PayPalPaymentSuccess?orderId={orderId}&customerId={customerId}"
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
                string paypalRedirectUrl = null;
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment  
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                //Process when Checkout with Paypal fails
                TempData["error"] = "Đặt hàng thất bại do thanh toán Paypal không thành công" + httpException.Message;
                return View("Failure");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaypalPaymentFail(string orderId, string customerId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if(order != null)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            TempData["error"] = "Đặt hàng thất bại do thanh toán Paypal không thành công";
            return View("Failure");
        }
        [HttpGet]
        public async Task<IActionResult> PayPalPaymentSuccess(string orderId, string customerId)
        {
            try
            {
                var cartItems = await _context.CartItems.Where(c => c.CustomerId == customerId).ToListAsync();
                if(cartItems.Count > 0)
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
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
