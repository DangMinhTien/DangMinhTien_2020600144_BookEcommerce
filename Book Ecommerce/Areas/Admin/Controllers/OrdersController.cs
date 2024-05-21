using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using System.Globalization;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("/quan-ly-don-hang")]
        public async Task<IActionResult> Index(string? search, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    ViewBag.search = search;
                }
                (var orderVMs, var pagingModel, var lstPageSize) = await _orderService.GetToViewManageAsync(search, page, pagesize);
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSizes = lstPageSize;
                ViewBag.pagesize = pagesize;
                return View(orderVMs);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/quan-ly-don-hang/chitiet")]
        public async Task<IActionResult> GetDetailOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderDetailToView(orderId);
                if (order == null)
                {
                    return BadRequest(new { mesClient = "Không tìm được đơn hàng cần xem", mesDev = "Order is not found" });
                }
                return Ok(new { order = order });
            }
            catch(Exception ex)
            {
                return BadRequest(new {mesClient = "Không lấy được thông tin đơn hàng do lỗi hệ thống", mesDev = ex.Message});
            }
        }
        [HttpPost("/quan-ly-don-hang/capnhattrangthai")]
        public async Task<IActionResult> UpdateStatus(string orderId, int status)
        {
            try
            {
                var order = await _orderService.GetSingleByConditionAsync(o => o.OrderId == orderId);
                if(order == null)
                {
                    return BadRequest(new { mesClient = "Không cập nhật được trạng thái do không tìm thấy đơn hàng", mesDev = "Order is not found" });
                }
                order.Status = status;
                await _orderService.UpdateAsync(order);
                string statusString = Generation.GenerationStatusOrderString(order.Status);
                TempData["success"] = $"Bạn vừa cập nhật trang thái đơn hàng {order.OrderCode} sang {statusString} thành công";
                return Ok(new
                {
                    mesClient = $"Bạn vừa cập nhật trang thái đơn hàng {order.OrderCode} sang {statusString} thành công",
                    mesDev = "Update order status successfuly"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {mesClient = "Cập nhật trạng thái đơn hàng thất bại do lỗi hệ thống", mesDev = ex.Message});
            }
        }
        [HttpPost("/quan-ly-don-hang/xoa")]
        public async Task<IActionResult> Delete(string orderId)
        {
            try
            {
                var order = await _orderService.GetSingleByConditionAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return BadRequest(new { mesClient = "Không xóa được đơn hàng do không tìm thấy đơn hàng", mesDev = "Order is not found" });
                }
                if(order.Status == (int)StatusOrder.GiaoThanhCong || order.Status == (int)StatusOrder.DangGiaoHang)
                {
                    var statusString = order.Status == (int)StatusOrder.GiaoThanhCong ? " đã giao thành công" : "đang giao hàng";
                    return BadRequest(new { mesClient = $"Không xóa được đơn hàng do đơn hàng {statusString}", mesDev = "status of order is not match" });
                }
                await _orderService.RemoveAsync(order);
                TempData["success"] = $"Bạn vừa xóa đơn hàng {order.OrderCode}";
                return Ok(new
                {
                    mesClient = $"Bạn vừa xóa đơn hàng {order.OrderCode}",
                    mesDev = "Delete order successfuly"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Xóa đơn hàng thất bại do lỗi hệ thống", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-don-hang/indonhang")]
        public async Task<IActionResult> GeneratePdf(string orderId)
        {
            try
            {
                var html = await _orderService.GenerateOrderToHtml(orderId);
                if (html == null)
                {
                    return BadRequest(new { mesClient = "Không xuất pdf được hóa đơn do không tìm thấy đơn hàng", mesDev = "Order is not found" });
                }
                HtmlToPdf oHtmlToPdf = new HtmlToPdf();
                PdfDocument oPdfDocument = oHtmlToPdf.ConvertHtmlString(html);
                byte[] pdf = oPdfDocument.Save();
                oPdfDocument.Close();
                return File(
                    pdf,
                    "application/pdf"
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không xuất được hóa đơn do lỗi hệ thống", mesDev = ex.Message });
            }
        }
    }
}
