using Book_Ecommerce.Domain.MySettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            TempData["success"] = "Chào mừng đến với trang quản trị";
            return View();
        }
    }
}
