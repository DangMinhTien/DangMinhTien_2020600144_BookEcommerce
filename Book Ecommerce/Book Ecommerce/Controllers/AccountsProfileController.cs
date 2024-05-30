using Book_Ecommerce.Data;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.AccountViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class AccountsProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly AppDbContext _dbContext;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountsProfileController(UserManager<AppUser> userManager,
            ICustomerService customerService,
            AppDbContext dbContext,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _customerService = customerService;
            _dbContext = dbContext;
            _signInManager = signInManager;
        }

        [HttpGet("/thong-tin-tai-khoan-khach-hang")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể mở được thông tin tài khoản do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể mở được thông tin tài khoản do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var accountVM = new AccountVM
                {
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = customer.FullName,
                    Gender = customer.Gender,
                    Address = customer.Address ?? "",
                    DateOfBirth = customer.DateOfBirth
                };
                return View(accountVM);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("/thong-tin-tai-khoan-khach-hang/capnhat")]
        public async Task<IActionResult> UpdateProfile(InputAccount inputAccount)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return BadRequest(new
                        {
                            mesClient = "Không thể thay đổi được thông tin tài khoản do không tìm thấy tài khoản đăng nhập",
                            mesDev = "Account not found"
                        });
                    }
                    var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                    if (customer == null)
                    {
                        return BadRequest(new
                        {
                            mesClient = "Không thể thay đổi được thông tin tài khoản do không tìm thấy nhân viên đăng nhập",
                            mesDev = "Account not found"
                        });
                    }
                    user.PhoneNumber = inputAccount.PhoneNumber;
                    customer.Address = inputAccount.Address;
                    customer.DateOfBirth = inputAccount.DateOfBirth ?? DateTime.Now;
                    customer.Gender = inputAccount.Gender;
                    customer.FullName = inputAccount.FullName;
                    await _dbContext.Database.BeginTransactionAsync();
                    await _userManager.UpdateAsync(user);
                    await _customerService.UpdateAsync(customer);
                    await _dbContext.Database.CommitTransactionAsync();
                    return Ok(new { mesClient = "Thay đổi thông tin tài khoản thành công", mesDev = "Update profile account successfully" });
                }
                catch (Exception ex)
                {
                    await _dbContext.Database.RollbackTransactionAsync();
                    return BadRequest(new { mesClient = "Không cập nhật được thông tin tài khoản do lỗi hệ thống", mesDev = ex.Message });
                }
            }
            List<string> error = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var err in value.Errors)
                {
                    error.Add(err.ErrorMessage);
                }
            }
            return BadRequest(new { error = error, isValid = false, mesClient = "Lỗi nhập dữ liệu", mesDev = "error input data" });
        }
        [HttpPost("/thong-tin-tai-khoan-khach-hang/doimatkhau")]
        public async Task<IActionResult> ChangePassword(InputChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return BadRequest(new
                        {
                            mesClient = "Không thể đổi mật khẩu của tài khoản do không tìm thấy tài khoản đăng nhập",
                            mesDev = "Account not found"
                        });
                    }
                    if (!(await _userManager.CheckPasswordAsync(user, changePassword.CurrentPassword)))
                    {
                        return BadRequest(new
                        {
                            mesClient = "Mật khẩu hiện tại bị sai",
                            mesDev = "Old is false"
                        });
                    }
                    var resuilt = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
                    if (!resuilt.Succeeded)
                    {
                        List<string> errorIdentity = new List<string>();
                        foreach (var err in resuilt.Errors)
                        {
                            errorIdentity.Add(err.Description);
                        }
                        return BadRequest(new { error = errorIdentity, isValid = false, mesClient = "Lỗi khi đổi mật khẩu", mesDev = "error input data" });
                    }
                    await _signInManager.SignOutAsync();
                    return Ok(new { mesClient = "Đổi mật khẩu tài khoản thành công", mesDev = "Update profile account successfully" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { mesClient = "Không đổi mật khẩu được tài khoản do lỗi hệ thống", mesDev = ex.Message });
                }
            }
            List<string> error = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var err in value.Errors)
                {
                    error.Add(err.ErrorMessage);
                }
            }
            return BadRequest(new { error = error, isValid = false, mesClient = "Lỗi nhập dữ liệu", mesDev = "error input data" });
        }
    }
}
