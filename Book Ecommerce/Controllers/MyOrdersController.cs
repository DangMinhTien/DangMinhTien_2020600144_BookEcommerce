using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class MyOrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;

        public MyOrdersController(IOrderService orderService,
            ICustomerService customerService,
            UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _customerService = customerService;
            _userManager = userManager;
        }
        [HttpGet("/don-hang-cua-toi")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể mở được danh sách đơn hàng do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể mở được danh sách đơn hàng do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                (var orderVMs, var pagingModel, var lstPageSize) = await _orderService.GetToViewMyOrderAsync(customer.CustomerId,
                                                                                                search, page, pagesize);
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSizes = lstPageSize;
                ViewBag.pagesize = pagesize;
                return View(orderVMs);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/don-hang-cua-toi/chitiet")]
        public async Task<IActionResult> GetDetailOrder(string orderId)
        {
            try
            {
                var order = await _orderService.Table().Include(o => o.OrderDetails)
                                                 .ThenInclude(od => od.Product)
                                                 .ThenInclude(p => p.Images)
                                                 .Include(o => o.Customer)
                                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                    return BadRequest(new { mesClient = "Không lấy được đơn hàng", mesDev = "Order is not found" });
                CultureInfo cultureInfo = new CultureInfo("vi-VN");
                var result = new
                {
                    orderId = order.OrderId,
                    orderCode = order.OrderCode,
                    dateCreated = order.DateCreated.ToString("dd/MM/yyyy"),
                    status = Generation.GenerationStatusOrderString(order.Status),
                    note = order.Note,
                    customerName = order.Customer.FullName,
                    fullName = order.FullName,
                    phoneNumber = order.PhoneNumber,
                    address = order.Address,
                    dateDelivery = order.DateDelivery.ToString("dd/MM/yyyy"),
                    paymentType = order.PaymentType,
                    transportFee = string.Format(cultureInfo, "{0:C0}", order.TransportFee),
                    totalAmount = string.Format(cultureInfo, "{0:C0}", order.OrderDetails.Sum(od => od.Quantity * od.Price) + order.TransportFee),
                    orderDetail = order.OrderDetails.Select(od => new
                    {
                        productName = od.Product.ProductName,
                        urlImage = od.Product.Images.FirstOrDefault()?.Url ?? "",
                        quantity = od.Quantity,
                        price = string.Format(cultureInfo, "{0:C0}", od.Price),
                        amount = string.Format(cultureInfo, "{0:C0}", od.Price * od.Quantity)
                    }).ToList(),
                };
                return Ok(new { order = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được thông tin đơn hàng do lỗi hệ thống", mesDev = ex.Message });
            }
        }
        [HttpPost("/don-hang-cua-toi/huydonhang")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetSingleByConditionAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return BadRequest(new { mesClient = "Không hủy được đơn hàng do không tìm thấy đơn hàng", mesDev = "Order is not found" });
                }
                if(order.Status == (int)StatusOrder.HuyDonHang)
                {
                    return BadRequest(new { mesClient = "Không hủy  được đơn hàng do đơn hàng đã được hủy", mesDev = "Order has cancel" });
                }
                if (order.Status == (int)StatusOrder.GiaoThanhCong || order.Status == (int)StatusOrder.DangGiaoHang)
                {
                    var statusString = order.Status == (int)StatusOrder.GiaoThanhCong ? " đã giao thành công" : "đang giao hàng";
                    return BadRequest(new { mesClient = $"Không hủy được đơn hàng do đơn hàng {statusString}", mesDev = "status of order is not match" });
                }
                order.Status = (int)StatusOrder.HuyDonHang;
                await _orderService.UpdateAsync(order);
                TempData["success"] = $"Bạn vừa hủy đơn hàng thành công";
                return Ok(new
                {
                    mesClient = $"Bạn vừa hủy đơn hàng thành công",
                    mesDev = "Update order status successfuly"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Hủy đơn hàng thất bại do lỗi hệ thống", mesDev = ex.Message });
            }
        }
    }
}
