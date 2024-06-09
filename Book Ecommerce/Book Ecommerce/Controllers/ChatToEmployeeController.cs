using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.ChatViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class ChatToEmployeeController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ChatToEmployeeController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("/chat-voi-nhan-vien")]
        public async Task<IActionResult> Index(string? search = null)
        {
            try
            {
                if (!_signInManager.IsSignedIn(User))
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                if(user.CustomerId == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập với tài khoản khách hàng để được tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _unitOfWork.CustomerRepository.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không tìm thấy khách hàng đang đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.customerId = customer.CustomerId;
                ViewBag.search = search;
                return View();
            }
            catch(Exception ex)
            {
                return NotFound("Lỗi hệ thống: " + ex.Message);
            }
        }
        [HttpGet("/chat-voi-nhan-vien/{employeeId}")]
        public async Task<IActionResult> ViewChat(string employeeId)
        {
            try
            {
                if (!_signInManager.IsSignedIn(User))
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập để được tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                if (user.CustomerId == null)
                {
                    TempData["error"] = "Bạn phải đăng nhập với tài khoản khách hàng để được tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _unitOfWork.CustomerRepository.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không tìm thấy khách hàng";
                    return RedirectToAction("Index", "Home");
                }
                var employee = await _unitOfWork.EmployeeRepository.GetSingleByConditionAsync(e => e.EmployeeId == employeeId);
                if (employee == null)
                {
                    TempData["error"] = "Không tìm thấy nhân viên tư vấn";
                    return RedirectToAction("Index", "Home");
                }
                var lstMessage = await _unitOfWork.MessageRepository.Table()
                            .Where(m => m.CustomerId == customer.CustomerId && m.EmployeeId == employeeId)
                            .OrderBy(m => m.SendDate)
                            .Select(m => new MessageVM
                            {
                                MessageId = m.MessageId,
                                CustomerId = m.CustomerId,
                                EmployeeId = m.EmployeeId,
                                Content = m.Content,
                                SendBy = m.SendBy,
                                SendDate = m.SendDate,
                            })
                            .ToListAsync();
                ViewBag.employeeId = employee.EmployeeId;
                ViewBag.customerId = customer.CustomerId;
                ViewBag.employee = employee;
                return View(lstMessage);
            }
            catch(Exception ex)
            {
                return NotFound("Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}
