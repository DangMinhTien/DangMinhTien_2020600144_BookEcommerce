using Book_Ecommerce.Data;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Services;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Book_Ecommerce.Infrastructure.Configuration
{
    public static class ConfigurationService
    {
        public static void RegisterDbContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<AppDbContext>(options =>
            {
                string connectString = configuration.GetConnectionString("Book_Ecommerce");
                options.UseSqlServer(connectString);
            });
        }
        public static void RegisterIdentityFramework(this IServiceCollection service)
        {
            service.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }
        public static void ConfigurationIdentity(this IServiceCollection service)
        {
            service.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
            });
        }
        public static void ConfigureApplicationCookie(this IServiceCollection service)
        {
            service.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/login/";
                option.LogoutPath = "/logout/";
                option.AccessDeniedPath = "/accessdenied/";
            });
        }
        public static void RegisterMailSender(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddOptions();
            var mailSetting = configuration.GetSection("MailSetting");
            service.Configure<MailSettingModel>(mailSetting);
            service.AddSingleton<IEmailSender, SendMailService>();
        }
        public static void RegisterAuthorization(this IServiceCollection service)
        {
            service.AddAuthorization(options =>
            {
                options.AddPolicy("IsCustomer", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole(MyRole.CUSTOMER);
                });
            });
        }
        public static void RegisterDI(this IServiceCollection service)
        {
            // đăng ký VnPay
            service.AddSingleton<IVnPayService, VnPayService>();
            // đăng ký message lỗi trong Identity
            service.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
            // Đăng ký PayPal
            service.AddScoped<IPayPalService, PayPalService>();
            // Đăng ký Cloudinary
            service.AddScoped<ICloudinaryService, CloudinaryService>();
            // Đăng ký DI
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            service.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            service.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<IBrandService, BrandService>();
            service.AddScoped<IAuthorService, AuthorService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<ICustomerService, CustomerService>();
            service.AddScoped<ICartService, CartService>();
            service.AddScoped<IProvinceService, ProvinceService>();
            service.AddScoped<ICheckoutService, CheckoutService>();
            service.AddScoped<IImageService, ImageService>();
            service.AddScoped<IAuthorProductService, AuthorProductService>();
            service.AddScoped<ICategoryProductService, CategoryProductService>();
        }
    }
}
