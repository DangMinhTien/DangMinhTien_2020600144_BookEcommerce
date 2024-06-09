using Microsoft.Extensions.FileProviders;
using Book_Ecommerce.Infrastructure.Configuration;
using Book_Ecommerce.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Đăng ký entity
builder.Services.RegisterDbContext(builder.Configuration);
// Đăng ký Identity
builder.Services.RegisterIdentityFramework();
// Các thiết lập với Identity
builder.Services.ConfigurationIdentity();
// cấu hình đăng nhập cookie
builder.Services.ConfigureApplicationCookie();
// đăng ký DI
builder.Services.RegisterDI();
//Đăng ký mail
builder.Services.RegisterMailSender(builder.Configuration);
// đăng ký ký phân quyền
builder.Services.RegisterAuthorization();

// đăng ký session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    // Cấu hình các tùy chọn khác cho Session
});
// đăng ký logging
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});
// đăng ký SignaIR
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
                    ),
    RequestPath = "/contents"
});
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<CommentHub>("/comment-hub");
app.MapHub<ChatHub>("/chat-hub");
app.Run();
