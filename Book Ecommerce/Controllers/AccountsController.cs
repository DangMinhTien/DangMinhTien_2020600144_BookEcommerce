using Book_Ecommerce.Models;
using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Book_Ecommerce.ExtendMethods;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Utilities;
using Book_Ecommerce.Data;
using Book_Ecommerce.Helpers;

namespace Book_Ecommerce.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;
        private SignInManager<AppUser> _signInManager;
        private UserManager<AppUser> _userManager;
        private IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManage;

        public AccountsController(AppDbContext context, 
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManage = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("/register")]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user != null)
            {
                // Người dùng tồn tại, bạn có thể tiếp tục thực hiện các thao tác khác với user
            }
            else
            {
                // Người dùng không tồn tại, xử lý trường hợp này theo ý bạn
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterVM registerVM, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            returnUrl = returnUrl ?? "/";
            if (ModelState.IsValid)
            {
                var codeNumber = _context.Customers.Count() > 0 ? _context.Customers.Max(c => c.CodeNumber) + 1 : 1000;
                var customer = new Customer
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    FullName = registerVM.FullName,
                    CodeNumber = codeNumber,
                    CustomerCode = "KH" + codeNumber,
                    Gender = registerVM.Gender,
                    Address = registerVM.Address,
                    DateOfBirth = registerVM.DateOfBirth,
                };
                try
                {
                    _context.Database.BeginTransaction();
                    var role = await _roleManage.FindByNameAsync(MyRole.CUSTOMER);
                    if (role == null)
                    {
                        role = new IdentityRole { Name = MyRole.CUSTOMER };
                        var roleResult = await _roleManage.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            _context.Database.RollbackTransaction();
                            TempData["error"] = "Không tạo được người dùng mới do không tìm thấy quyền khách hàng";
                            return View();
                        }
                    }
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                    var user = new AppUser
                    {
                        UserName = registerVM.Email,
                        Email = registerVM.Email,
                        PhoneNumber = registerVM.PhoneNumber,
                        CustomerId = customer.CustomerId
                    };
                    var result = await _userManager.CreateAsync(user, registerVM.Password);
                    if (result.Succeeded)
                    {
                        // Phát sinh token để xác nhận email
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // https://localhost:5001/confirm-email?userId=fdsfds&code=xyz&returnUrl=
                        var callbackUrl = Url.ActionLink(
                            action: nameof(ConfirmEmail),
                            values:
                                new
                                {
                                    userId = user.Id,
                                    code = code
                                });

                        await _emailSender.SendEmailAsync(registerVM.Email,
                            "Xác nhận địa chỉ email",
                            @$"Bạn đã đăng ký tài khoản trên Book_Ecommerce,
                           hãy <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a> 
                           để kích hoạt tài khoản.");
                        var addToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                        if (!addToRoleResult.Succeeded)
                        {
                            _context.Database.RollbackTransaction();
                            TempData["error"] = "Không tạo được người dùng mới do không thêm được quyền cho ài khoản";
                            return View();
                        }
                        await _context.Database.CommitTransactionAsync();
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            TempData["infor"] = "Hãy kiểm tra hòm thư để xác thực tài khoản";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            TempData["success"] = "Đăng ký tài khoản thành công";
                            return LocalRedirect(returnUrl);
                        }

                    }
                    await _context.Database.RollbackTransactionAsync();
                    ModelState.AddModelError(result);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    await _context.Database.RollbackTransactionAsync();
                }
            }
            TempData["error"] = "Lỗi khi tạo tài khoản";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("ErrorConfirmEmail");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    TempData["error"] = $"Xác thực email không thành công";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    var result = await _userManager.ConfirmEmailAsync(user, code);
                    if (result.Succeeded)
                        TempData["success"] = $"Xác thực email thành công cho tài khoản {user.UserName}";
                    else
                        TempData["error"] = $"Xác thực email không thành công cho tài khoản {user.UserName}";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                TempData["error"] = "Xác thực email không thành công";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet("/login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            returnUrl = returnUrl ?? "/";
            if (ModelState.IsValid)
            {

                
                // Tìm UserName theo Email, đăng nhập lại
                var user = await _userManager.FindByNameAsync(loginVM.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email không tồn tại tài khoản");
                    return View(loginVM);
                }
                if(!await _userManager.CheckPasswordAsync(user, loginVM.Password))
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu bị sai");
                    return View(loginVM);
                }
                if(user.EmailConfirmed == false)
                {
                    TempData["error"] = "Đăng nhập thất bại bạn chưa xác thực email";
                    return View(loginVM);
                }
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, true, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    TempData["success"] = $"Tài khoản {user.UserName} đăng nhập thành công !";
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    TempData["error"] = "Đăng nhập thất bại dó yêu cầu xác thực 2 bước";
                    return View(loginVM);
                }

                if (result.IsLockedOut)
                {
                    TempData["error"] = "Tài khoản của bạn đang bị khóa";
                    return View(loginVM);
                }
                else
                {
                    TempData["error"] = "Không đăng nhập được";
                    return View(loginVM);
                }
            }
            return View(loginVM);
        }
        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Đăng xuất thành công !";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet("/accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet("/forgotpassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost("/forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, $"không có tài khoản nào sử dụng email {model.Email}");
                    return View(model);
                }
                if(!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError(string.Empty, $"Tài khoản của bạn chưa được xác thực email nên không thể đổi mật khẩu");
                    return View(model);
                }
                var key = Encription.GenerateKey();
                var passwordHash = Encription.Encrypt(model.Password, key);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.ActionLink(
                    action: nameof(ResetPassword),
                    values: new { userId = user.Id, code = code, key = key, passwordHash = passwordHash},
                    protocol: Request.Scheme);


                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Xác nhận thay đổi mật khẩu",
                    $"Hãy bấm <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>vào đây</a> để xác nhận thay đổi mật khẩu.");
                TempData["infor"] = "Hãy xác nhận thay đổi mật khẩu trong email của bạn";
                return View(model);
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string code, string key, string passwordHash)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["error"] = "Xác nhận thay đổi mật khẩu thất bại do không tìm thấy tài khoản của bạn";
                    return View("ForgotPassword");
                }
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var password = Encription.Decrypt(passwordHash, key);

                var result = await _userManager.ResetPasswordAsync(user, code, password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Xác nhận thay đổi mật khẩu thành công";
                    return RedirectToAction("Index", "Home");
                }
                TempData["error"] = "Xác nhận thay đổi mật khẩu thất bại";
                return View("ForgotPassword");

            }
            catch
            {
                TempData["error"] = "Xác nhận thay đổi mật khẩu thất bại";
                return View("ForgotPassword");
            }
        }
    }
}
