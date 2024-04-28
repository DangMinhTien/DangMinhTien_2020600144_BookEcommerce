using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            TempData["success"] = "Chào mừng đến với trang quản trị";
            return View();
        }
    }
}
