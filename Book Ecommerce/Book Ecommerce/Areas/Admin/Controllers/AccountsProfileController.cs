using Book_Ecommerce.Data;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.AccountViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class AccountsProfileController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeService _employeeService;
        private readonly AppDbContext _dbContext;

        public AccountsProfileController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IEmployeeService employeeService,
            AppDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _employeeService = employeeService;
            _dbContext = dbContext;
        }
        [HttpGet("/thong-tin-tai-khoan-nhan-vien")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể mở được thông tin tài khoản do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction("Index", "Home", new {area = "Admin"});
                }
                var employee = await _employeeService.GetSingleByCondionAsync(e => e.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    TempData["error"] = "Không thể mở được thông tin tài khoản do không tìm thấy nhân viên đăng nhập";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var accountVM = new AccountVM
                {
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = employee.FullName,
                    Gender = employee.Gender,
                    Address = employee.Address,
                    DateOfBirth = employee.DateOfBirth
                };
                return View(accountVM);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("/thong-tin-tai-khoan-nhan-vien/capnhat")]
        public async Task<IActionResult> UpdateProfile(InputAccount inputAccount)
        {
            if(ModelState.IsValid)
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
                    var employee = await _employeeService.GetSingleByCondionAsync(e => e.EmployeeId == user.EmployeeId);
                    if (employee == null)
                    {
                        return BadRequest(new
                        {
                            mesClient = "Không thể thay đổi được thông tin tài khoản do không tìm thấy nhân viên đăng nhập",
                            mesDev = "Account not found"
                        });
                    }
                    user.PhoneNumber = inputAccount.PhoneNumber;
                    employee.Address = inputAccount.Address;
                    employee.DateOfBirth = inputAccount.DateOfBirth ?? DateTime.Now;
                    employee.Gender = inputAccount.Gender;
                    employee.FullName = inputAccount.FullName;
                    await _dbContext.Database.BeginTransactionAsync();
                    await _userManager.UpdateAsync(user);
                    await _employeeService.UpdateAsync(employee);
                    await _dbContext.Database.CommitTransactionAsync();
                    return Ok(new {mesClient = "Thay đổi thông tin tài khoản thành công", mesDev = "Update profile account successfully"});
                }
                catch(Exception ex)
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
        [HttpPost("/thong-tin-tai-khoan-nhan-vien/doimatkhau")]
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
                    if(!(await _userManager.CheckPasswordAsync(user, changePassword.CurrentPassword)))
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
