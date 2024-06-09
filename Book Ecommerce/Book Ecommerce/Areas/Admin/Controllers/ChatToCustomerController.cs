using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.ChatViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class ChatToCustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public ChatToCustomerController(IUnitOfWork unitOfWork,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("/chat-voi-khach-hang")]
        public async Task<IActionResult> Index(string? search = null)
        {
            try
            {
                if (!_signInManager.IsSignedIn(User))
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home", new {area = "Admin"});
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (user.EmployeeId == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập với tài khoản khách hàng để được tư vấn";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var employee = await _unitOfWork.EmployeeRepository.GetSingleByConditionAsync(e => e.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    TempData["error"] = "Không tìm thấy nhân viên đang đăng nhập";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                ViewBag.employeeId = employee.EmployeeId;
                ViewBag.search = search;
                return View();
            }
            catch(Exception ex)
            {
                return NotFound("Lỗi hệ thống: " + ex.Message);
            }
        }
        [HttpGet("/chat-voi-khach-hang/{customerId}")]
        public async Task<IActionResult> ViewChat(string customerId)
        {
            try
            {
                if (!_signInManager.IsSignedIn(User))
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (user.EmployeeId == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập với tài khoản khách hàng để được tư vấn";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var employee = await _unitOfWork.EmployeeRepository.GetSingleByConditionAsync(e => e.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    TempData["error"] = "Không tìm thấy nhân viên đang đăng nhập";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                var customer = await _unitOfWork.CustomerRepository.GetSingleByConditionAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    TempData["error"] = "Không tìm thấy khách hàng tư vấn";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var lstMessage = await _unitOfWork.MessageRepository.Table()
                            .Where(m => m.CustomerId == customerId && m.EmployeeId == employee.EmployeeId)
                            .OrderBy(m => m.SendDate)
                            .Select(m => new MessageVM
                            {
                                MessageId = m.MessageId,
                                CustomerId = m.CustomerId,
                                EmployeeId = m.EmployeeId,
                                Content = m.Content,
                                SendBy = m.SendBy,
                                SendDate = m.SendDate,
                            }).ToListAsync();

                ViewBag.employeeId = employee.EmployeeId;
                ViewBag.customerId = customer.CustomerId;
                ViewBag.customer = customer;
                return View(lstMessage);
            }
            catch (Exception ex)
            {
                return NotFound("Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}
