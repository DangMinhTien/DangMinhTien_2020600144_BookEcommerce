using Book_Ecommerce.Data;
using Book_Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signManager;
        private readonly UserManager<AppUser> _userManager;

        public CartController(AppDbContext context,
                                SignInManager<AppUser> signInManager,
                                UserManager<AppUser> userManager)
        {
            _context = context;
            _signManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return BadRequest(new { mesclient = "Không tìm thấy tài khoản đăng nhập", mesdev = "Account not found" });
                }
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == user.CustomerId);
                if(customer == null)
                {
                    return BadRequest(new { mesclient = "Không tìm thấy khách hàng đang đăng nhập", mesdev = "Customer not found" });
                }
                if(quantity < 1)
                {
                    return BadRequest(new
                    {
                        mesclient = "Thêm vào giỏ hàng thất bại do số lượng thêm vào nhỏ hơn 1",
                        mesdev = "Quantity is smaller than 1"
                    });
                }
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                if(product == null)
                {
                    return BadRequest(new
                    {
                        mesclient = "Thêm vào giỏ hàng thất bại do không tìm thấy sản phẩm cần thêm",
                        mesdev = "Product is not found"
                    });
                }    
                var cartItem = await _context.CartItems.
                    FirstOrDefaultAsync(c => c.ProductId == productId 
                                    && c.CustomerId == customer.CustomerId);
                var quantityInCartItem = cartItem == null ? 0 : cartItem.Quantity;
                if(quantityInCartItem + quantity > product.Quantity)
                {
                    return BadRequest(new
                    {
                        mesclient = "Thêm vào giỏ hàng thất bại do số lượng bị vượt quá số lượng của sản phẩm",
                        mesdev = "Quantity + QuantityInCartitem is bigger than quantityInProduct"
                    });
                }
                if(cartItem == null)
                {
                    var newCartItem = new CartItem
                    {
                        ProductId = productId,
                        CustomerId = customer.CustomerId,
                        Quantity = quantity,
                    };
                    _context.CartItems.Add(newCartItem);
                }
                else
                {
                    cartItem.Quantity = quantityInCartItem + quantity;
                    _context.CartItems.Update(cartItem);
                }
                await _context.SaveChangesAsync();
                return Json(new 
                {
                    mesclient = "Thêm vào giỏ hàng thành công", 
                    mesdev = "Add to cart is success",
                    sumitem = _context.CartItems.Where(c => c.CustomerId == customer.CustomerId).Sum(c => c.Quantity),
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new {mesclient = "Thêm vào giỏ hàng thất bại", mesdev = ex.Message});
            }
        }
    }
}
