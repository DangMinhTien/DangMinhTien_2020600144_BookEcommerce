using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class FavouritesController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IFavouriteProductService _favoriteProductService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;

        public FavouritesController(ICustomerService customerService,
            IFavouriteProductService favouriteProductService,
            UserManager<AppUser> userManager,
            IProductService productService)
        {
            _customerService = customerService;
            _favoriteProductService = favouriteProductService;
            _userManager = userManager;
            _productService = productService;
        }
        [HttpGet("/danh-sach-yeu-thich")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể mở được danh sách yêu thích do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể mở được danh sách yêu thích do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var favorites = await _favoriteProductService.Table().Include(fp => fp.Product)
                                                        .ThenInclude(p => p.Images)
                                                        .Where(fp => fp.CustomerId == customer.CustomerId)
                                                        .ToListAsync();
                var productVMs = favorites.Select(fp => new ProductVM
                {
                    ProductId = fp.Product.ProductId,
                    ProductName = fp.Product.ProductName,
                    ProductCode = fp.Product.ProductCode,
                    ProductSlug = fp.Product.ProductSlug,
                    Price = fp.Product.Price,
                    PercentDiscount = fp.Product.PercentDiscount,
                    PriceAfterDiscount = (fp.Product.PercentDiscount > 0) ? 
                        fp.Product.Price - (fp.Product.Price * ((decimal)fp.Product.PercentDiscount / 100)) 
                        : fp.Product.Price,
                    Images = fp.Product.Images,
                }).ToList();
                return View(productVMs);
            }
            catch(Exception ex)
            {
                TempData["error"] = "Không thể mở được danh sách yêu thích" + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("/danh-sach-yeu-thich/themyeuthich")]
        public async Task<IActionResult> AddToFavourite(string productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy tài khoản đăng nhập", mesDev = "Account not found" });
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy khách hàng đang đăng nhập", mesDev = "Customer not found" });
                }
                var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    return BadRequest(new
                    {
                        mesClient = "Thêm vào giỏ hàng thất bại do không tìm thấy sản phẩm cần thêm",
                        mesDev = "Product is not found"
                    });
                }
                var favoriteProduct = await _favoriteProductService
                    .GetSingleByConditionAsync(fp => fp.ProductId == product.ProductId 
                        && fp.CustomerId == customer.CustomerId);
                if(favoriteProduct != null)
                {
                    return BadRequest(new { mesClient = "Sản phẩm này đã trong danh sách yêu thích", mesDev = "favoriteProduct has contain" });
                }
                await _favoriteProductService.AddAsync(new FavouriteProduct
                {
                    ProductId = product.ProductId,
                    CustomerId = customer.CustomerId,
                });
                return Ok(new
                {
                    mesClient = "Thêm vào danh sách yêu thích thành công",
                    mesDev = "Add to list favorites is successfully",
                    sumItem = _favoriteProductService.Table().Count(fp => fp.CustomerId == customer.CustomerId),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Thêm vào giỏ hàng thất bại", mesDev = ex.Message });
            }
        }
        [HttpPost("/danh-sach-yeu-thich/xoa")]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể xóa được sản phẩm trong danh sách yêu thích do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction(nameof(Index));
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể xóa được sản phẩm trong danh sách yêu thích do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction(nameof(Index));
                }
                var favourite = await _favoriteProductService.GetSingleByConditionAsync(fp => fp.CustomerId == customer.CustomerId && fp.ProductId == productId);
                if (favourite == null)
                {
                    TempData["error"] = "Không thể xóa được sản phẩm trong danh sách yêu thích do không tìm thấy sản phẩm";
                    return RedirectToAction(nameof(Index));
                }
                await _favoriteProductService.RemoveAsync(favourite);
                TempData["success"] = "Xóa sản phẩm trong danh sách yêu thích thành công";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["error"] = "Không thể xóa được sản phẩm trong danh sách yêu thích";
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
