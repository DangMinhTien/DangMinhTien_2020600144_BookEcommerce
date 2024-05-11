using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;

        public CustomersController(ICustomerService customerService,
            UserManager<AppUser> userManager)
        {
            _customerService = customerService;
            _userManager = userManager;
        }
        [HttpGet("/quan-ly-khach-hang")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    ViewBag.search = search;
                }
                (var customerVMs, var pagingModel, var lstPageSize) = await _customerService.GetToViewManageAsync(search, page, pagesize);
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSizes = lstPageSize;
                ViewBag.pagesize = pagesize;
                return View(customerVMs);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("/quan-ly-khach-hang/capnhattrangthai")]
        public async Task<IActionResult> UpdateStatus(string customerId)
        {
            try
            {
                var customer = await _customerService.Table().Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy khách hàng cần thay đổi trạng thái", mesDev = "Customer not found" });
                }
                if (customer.User == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy tài khoản khách hàng cần thay đổi trạng thái", mesDev = "Customer Account not found" });
                }
                if (customer.User.LockoutEnd == null)
                {
                    var resultLock = await _userManager.SetLockoutEndDateAsync(customer.User, DateTimeOffset.MaxValue);
                    if (!resultLock.Succeeded)
                    {
                        return BadRequest(new { mesClient = "Không thể khóa được tài khoản khách hàng", mesDev = resultLock.Errors.Select(e => e.Description).ToList() });
                    }
                }
                else
                {
                    var resultUnLock = await _userManager.SetLockoutEndDateAsync(customer.User, null);
                    if (!resultUnLock.Succeeded)
                    {
                        return BadRequest(new { mesClient = "Không thể mở khóa được tài khoản khách hàng", mesDev = resultUnLock.Errors.Select(e => e.Description).ToList() });
                    }
                }
                TempData["success"] = "Thay đổi trạng thái tài khoản khách hàng thành công";
                return Ok(new { mesClient = "Thay đổi trạng thái tài khoản khách hàng thành công", mesDev = "Update status customer account successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không thể thay đổi trạng thái tài khoản khách hàng do hệ thống lỗi", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-khach-hang/xoa")]
        public async Task<IActionResult> Delete(string customerId)
        {
            try
            {
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy khách hàng cần xóa", mesDev = "Customer not found" });
                }
                await _customerService.RemoveAsync(customer);
                TempData["success"] = "Xóa khách hàng thành công";
                return Ok(new { mesClient = "Xóa khách hàng thành công", mesDev = "Delete customer successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không thể xóa khách hàng do hệ thống lỗi", mesDev = ex.Message });
            }
        }
    }
}
