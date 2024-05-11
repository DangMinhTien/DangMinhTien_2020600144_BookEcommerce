using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Book_Ecommerce.Domain.ViewModels.EmployeeViewModel;
using Microsoft.AspNetCore.Identity;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels.UserViewModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text;
using Book_Ecommerce.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Book_Ecommerce.Domain.ExtendMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging.Abstractions;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = MyRole.ADMIN)]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private UserManager<AppUser> _userManager;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesController(IEmployeeService employeeService,
            UserManager<AppUser> userManager,
            IUserService userService,
            IEmailSender emailSender,
            AppDbContext dbContext,
            RoleManager<IdentityRole> roleManager)
        {
            _employeeService = employeeService;
            _userManager = userManager;
            _userService = userService;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }
        [HttpGet("/quan-ly-nhan-vien")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    ViewBag.search = search;
                }
                (var employeeVMs, var pagingModel, var lstPageSize) = await _employeeService.GetToViewManageAsync(search, page, pagesize);
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSizes = lstPageSize;
                ViewBag.pagesize = pagesize;
                // lấy role
                var lstRole = await _roleManager.Roles.Where(r => r.Name != MyRole.CUSTOMER).Select(r => r.Name).ToListAsync();
                var roleNames = new string[] { MyRole.EMPLOYEE, MyRole.ADMIN };
                var roleAdds = roleNames.Where(r => !lstRole.Contains(r)).ToList();
                if(roleAdds.Count > 0)
                {
                    foreach(var roleAdd in roleAdds)
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleAdd});
                    }
                    lstRole = await _roleManager.Roles.Where(r => r.Name != MyRole.CUSTOMER).Select(r => r.Name).ToListAsync();
                }
                ViewBag.roleNames = new MultiSelectList(lstRole);
                return View(employeeVMs);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("/quan-ly-nhan-vien/themmoi")]
        public async Task<IActionResult> Create(InputEmployee inputEmployee)
        {
            if(_userService.Table().Any(u => u.Email == inputEmployee.Email))
            {
                ModelState.AddModelError(string.Empty, "Email bị trùng với một tài khoản khác");
            }
            if(_userService.Table().Any(u => u.PhoneNumber == inputEmployee.PhoneNumber))
            {
                ModelState.AddModelError(string.Empty, "Số điện thoại bị trùng với một tài khoản khác");
            }
            if(ModelState.IsValid)
            {
                try
                {
                    await _dbContext.Database.BeginTransactionAsync();
                    var role = await _roleManager.FindByNameAsync(MyRole.EMPLOYEE);
                    if (role == null)
                    {
                        role = new IdentityRole { Name = MyRole.EMPLOYEE };
                        var roleResult = await _roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            await _dbContext.Database.RollbackTransactionAsync();
                            return BadRequest(new
                            {
                                isValid = true,
                                mesClient = "Không thêm được nhân viên do không tìm được quyền nhân viên",
                                mesDev = "Role Employee not found"
                            });
                        }
                    }
                    (var result, var user, var employee) = await _userService.RegisterEmployeeAccountAsync(inputEmployee);
                    if (result.Succeeded)
                    {
                        // Phát sinh token để xác nhận email
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // https://localhost:5001/confirm-email?userId=fdsfds&code=xyz&returnUrl=
                        var callbackUrl = Url.ActionLink(
                            action: "ConfirmEmail",
                            controller: "Accounts",
                            values:
                                new
                                {
                                    area = "",
                                    userId = user.Id,
                                    code = code
                                });

                        await _emailSender.SendEmailAsync(inputEmployee.Email,
                            "Xác nhận địa chỉ email",
                            @$"Bạn đã đăng ký tài khoản nhân viên ({employee.EmployeeCode}) trên Hệ thống nhà sách MinhTienBook,
                           hãy <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a> 
                           để kích hoạt tài khoản.");
                        var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                        if (!addToRoleResult.Succeeded)
                        {
                            await _dbContext.Database.RollbackTransactionAsync();
                            return BadRequest(new
                            {
                                isValid = true,
                                mesClient = "Không tạo được nhân viên mới do không thêm được quyền cho tài khoản",
                                mesDev = "Add Role is failure"
                            });
                        }
                        await _dbContext.Database.CommitTransactionAsync();
                        TempData["success"] = "Thêm nhân viên thành công";
                        return Ok(new { mesClient = "Thêm nhân viên thành công", mesDev = "Add Employee successfully" });
                    }
                    await _dbContext.Database.RollbackTransactionAsync();
                    ModelState.AddModelError(result);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    await _dbContext.Database.RollbackTransactionAsync();
                    return BadRequest(new
                    {
                        isValid = true,
                        mesClient = "Không thêm được nhân viên do lỗi hệ thống",
                        mesDev = ex.Message
                    });
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
        [HttpGet("/quan-ly-nhan-vien/quyen")]
        public async Task<IActionResult> GetRoles(string employeeId)
        {
            try
            {
                var employee = await _employeeService.Table().Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
                if (employee == null)
                {
                    return BadRequest(new {mesClient = "Không tìm thấy nhân viên cần phân quyền", mesDev = "Employee not found"});
                }
                if(employee.User == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy tài khoản nhân viên cần phân quyền", mesDev = "Employee Account not found" });
                }
                var roleNames = (await _userManager.GetRolesAsync(employee.User)).ToList();
                return Ok(new {roleNames = roleNames});
            }
            catch(Exception ex)
            {
                return BadRequest(new { mesClient = "Có lỗi khi lấy quyền của tài khoản", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-nhan-vien/phanquyen")]
        public async Task<IActionResult> UpdateRoles(string employeeId, List<string> roleNames = null!)
        {
            if(roleNames == null || roleNames.Count == 0)
            {
                ModelState.AddModelError("Bạn phải chọn quyền cho nhân viên");
            }
            if (roleNames != null && roleNames.Count > 0 && !roleNames.Contains(MyRole.EMPLOYEE))
            {
                ModelState.AddModelError("Bạn phải chọn quyền Nhân viên cho nhân viên");
            }
            if (roleNames != null && roleNames.Count > 0 && roleNames.Contains(MyRole.CUSTOMER))
            {
                ModelState.AddModelError("Bạn không được chọn quyền Khách hàng cho nhân viên");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (roleNames == null || roleNames.Count == 0)
                    {
                        return BadRequest(new { mesClient = "Cập nhật thất bại do chưa chọn quyền", mesDev = "roleNames is not found" });
                    }
                    var employee = await _employeeService.Table().Include(e => e.User)
                        .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
                    if (employee == null)
                    {
                        return BadRequest(new { mesClient = "Không tìm thấy nhân viên cần phân quyền", mesDev = "Employee not found" });
                    }
                    if (employee.User == null)
                    {
                        return BadRequest(new { mesClient = "Không tìm thấy tài khoản nhân viên cần phân quyền", mesDev = "Employee Account not found" });
                    }
                    var oldRoleNames = (await _userManager.GetRolesAsync(employee.User)).ToList();
                    var deleteRoleNames = oldRoleNames.Where(or => !roleNames.Contains(or)).ToList();
                    var addRoleNames = roleNames.Where(r => !oldRoleNames.Contains(r)).ToList();
                    await _dbContext.Database.BeginTransactionAsync();
                    if (deleteRoleNames.Count > 0)
                    {
                        var resultDelete = await _userManager.RemoveFromRolesAsync(employee.User, deleteRoleNames);
                        if (!resultDelete.Succeeded)
                        {
                            await _dbContext.Database.RollbackTransactionAsync();
                            return BadRequest(new { mesClient = "Lỗi phân quyền khi không thể xóa được các quyền cũ của tài khoản", mesDev = "Can not delete old role" });
                        }
                    }
                    if (addRoleNames.Count > 0)
                    {
                        var resultAdd = await _userManager.AddToRolesAsync(employee.User, addRoleNames);
                        if (!resultAdd.Succeeded)
                        {
                            await _dbContext.Database.RollbackTransactionAsync();
                            return BadRequest(new { mesClient = "Lỗi phân quyền khi không thể thêm được các quyền mới của tài khoản", mesDev = "Can not add new role" });
                        }
                    }
                    await _dbContext.Database.CommitTransactionAsync();
                    TempData["success"] = "Cập nhật quyền thành công";
                    return Ok(new { mesClient = "Cập nhật quyền thành công", mesDev = "Update role employee successfully" });
                }
                catch(Exception ex)
                {
                    await _dbContext.Database.RollbackTransactionAsync();
                    return BadRequest(new { mesClient = "Không thể phân quyền do hệ thống lỗi", mesDev = ex.Message });
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
        [HttpPost("/quan-ly-nhan-vien/capnhattrangthai")]
        public async Task<IActionResult> UpdateStatus(string employeeId)
        {
            try
            {
                var employee = await _employeeService.Table().Include(e => e.User)
                        .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
                if (employee == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy nhân viên cần thay đổi trạng thái", mesDev = "Employee not found" });
                }
                if (employee.User == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy tài khoản nhân viên cần thay đổi trạng thái", mesDev = "Employee Account not found" });
                }
                if(employee.User.LockoutEnd == null)
                {
                    var resultLock = await _userManager.SetLockoutEndDateAsync(employee.User, DateTimeOffset.MaxValue);
                    if (!resultLock.Succeeded)
                    {
                        return BadRequest(new { mesClient = "Không thể khóa được tài khoản nhân viên", mesDev = resultLock.Errors.Select(e => e.Description).ToList() });
                    }
                }
                else
                {
                    var resultUnLock = await _userManager.SetLockoutEndDateAsync(employee.User, null);
                    if (!resultUnLock.Succeeded)
                    {
                        return BadRequest(new { mesClient = "Không thể mở khóa được tài khoản nhân viên", mesDev = resultUnLock.Errors.Select(e => e.Description).ToList() });
                    }
                }
                TempData["success"] = "Thay đổi trạng thái tài khoản nhân viên thành công";
                return Ok(new { mesClient = "Thay đổi trạng thái tài khoản nhân viên thành công", mesDev = "Update status employee account successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { mesClient = "Không thể thay đổi trạng thái tài khoản nhân viên do hệ thống lỗi", mesDev = ex.Message });
            }
        }
        public async Task<IActionResult> Delete(string employeeId)
        {
            try
            {
                var employee = await _employeeService.GetSingleByCondionAsync(e => e.EmployeeId == employeeId);
                if (employee == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy nhân viên cần xóa", mesDev = "Employee not found" });
                }
                await _employeeService.RemoveAsync(employee);
                TempData["success"] = "Xóa nhân viên thành công";
                return Ok(new { mesClient = "Xóa nhân viên thành công", mesDev = "Delete employee successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không thể xóa nhân viên do hệ thống lỗi", mesDev = ex.Message });
            }
        }
    }
}
