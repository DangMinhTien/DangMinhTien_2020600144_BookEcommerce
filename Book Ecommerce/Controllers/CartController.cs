using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;

namespace Book_Ecommerce.Controllers
{
    [Authorize(Roles = MyRole.CUSTOMER)]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        CultureInfo cultureInfo = new CultureInfo("vi-VN");

        public CartController(AppDbContext context,
                                SignInManager<AppUser> signInManager,
                                UserManager<AppUser> userManager,
                                ICartService cartService,
                                ICustomerService customerService,
                                IProductService productService)
        {
            _context = context;
            _signManager = signInManager;
            _userManager = userManager;
            _cartService = cartService;
            _customerService = customerService;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể mở được giỏ hàng do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể mở được giỏ hàng do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction("Index", "Home");
                }
                var cartItemVMs = await _cartService.GetByCustomerToViewAsync(customer.CustomerId);
                if(cartItemVMs == null || cartItemVMs.Count() == 0)
                {
                    TempData["infor"] = "Giỏ hàng của khách hàng đang trống";
                    return RedirectToAction("Index", "Home");
                }
                return View(cartItemVMs);
            }
            catch
            {
                TempData["error"] = "Không thể mở được giỏ hàng";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy tài khoản đăng nhập", mesDev = "Account not found" });
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if(customer == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy khách hàng đang đăng nhập", mesDev = "Customer not found" });
                }
                if(quantity < 1)
                {
                    return BadRequest(new
                    {
                        mesClient = "Thêm vào giỏ hàng thất bại do số lượng thêm vào nhỏ hơn 1",
                        mesDev = "Quantity is smaller than 1"
                    });
                }
                var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == productId);
                if(product == null)
                {
                    return BadRequest(new
                    {
                        mesClient = "Thêm vào giỏ hàng thất bại do không tìm thấy sản phẩm cần thêm",
                        mesDev = "Product is not found"
                    });
                }    
                var cartItem = await _cartService.GetSingleByConditionAsync(c => c.ProductId == productId 
                                    && c.CustomerId == customer.CustomerId);
                var quantityInCartItem = cartItem == null ? 0 : cartItem.Quantity;
                if(quantityInCartItem + quantity > product.Quantity)
                {
                    return BadRequest(new
                    {
                        mesClient = "Thêm vào giỏ hàng thất bại do số lượng bị vượt quá số lượng của sản phẩm",
                        mesDev = "Quantity + QuantityInCartitem is bigger than quantityInProduct"
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
                    await _cartService.AddAsync(newCartItem);
                }
                else
                {
                    cartItem.Quantity = quantityInCartItem + quantity;
                    await _cartService.UpdateAsync(cartItem);
                }
                return Ok(new 
                {
                    mesClient = "Thêm vào giỏ hàng thành công", 
                    mesDev = "Add to cart is success",
                    sumItem = _context.CartItems.Where(c => c.CustomerId == customer.CustomerId).Sum(c => c.Quantity),
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new {mesClient = "Thêm vào giỏ hàng thất bại", mesDev = ex.Message});
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCart(string productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "Không thể xóa được sản phẩm trong giỏ hàng do không tìm thấy tài khoản đăng nhập";
                    return RedirectToAction(nameof(Index));
                }
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    TempData["error"] = "Không thể xóa được sản phẩm trong giỏ hàng do không tìm thấy khách hàng đăng nhập";
                    return RedirectToAction(nameof(Index));
                }
                var cartItem = await _cartService.GetSingleByConditionAsync(c => c.CustomerId == customer.CustomerId && c.ProductId == productId);
                if(cartItem == null)
                {
                    TempData["error"] = "Không thể xóa được sản phẩm trong giỏ hàng do không tìm thấy giỏ hàng";
                    return RedirectToAction(nameof(Index));
                }
                await _cartService.RemoveAsync(cartItem);
                TempData["success"] = "Xóa sản phẩm trong giỏ hàng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["error"] = "Không thể xóa được sản phẩm trong giỏ hàng";
                return RedirectToAction(nameof(Index));
            }

        }
        [HttpPost]
        public async Task<IActionResult> PlusCart(string productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Tăng số lượng thất bại do không tìm thấy tài khoản đăng nhập", 
                        mesDev = "Account not found" 
                    });
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Tăng số lượng thất bại do không tìm thấy khách hàng đang đăng nhập", 
                        mesDev = "Customer not found" 
                    });
                }
                var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Tăng số lượng thất bại do không tìm thấy sản phẩm cần thêm", 
                        mesDev = "Product not found" 
                    });
                }
                var cartItem = await _cartService.GetSingleByConditionAsync(c => c.CustomerId == customer.CustomerId && c.ProductId == productId);
                if(cartItem == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Tăng số lượng thất bại do không tìm thấy giỏ hàng", 
                        mesDev = "Cart not found" 
                    });
                }
                if(cartItem.Quantity + 1 > product.Quantity)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Tăng số lượng thất bại do số lượng sản phẩm không đủ", 
                        mesDev = "quantity in cart is bigger than product's quantity" 
                    });
                }
                cartItem.Quantity = cartItem.Quantity + 1;
                await _cartService.UpdateAsync(cartItem);
                var priceItem = (product.PercentDiscount == null || product.PercentDiscount == 0) ? product.Price
                                    : product.Price - (product.Price * (decimal)product.PercentDiscount / 100);
                var cart = await _cartService.Table()
                                            .Include(c => c.Product)
                                            .Where(c => c.CustomerId == customer.CustomerId)
                                            .ToListAsync();
                var amount = cart.Sum(c =>
                {
                    var price = (c.Product.PercentDiscount == null || c.Product.PercentDiscount == 0) ? c.Product.Price
                                    : c.Product.Price - (c.Product.Price * (decimal)c.Product.PercentDiscount / 100);
                    return c.Quantity * price;
                });
                return Ok(new
                {
                    mesClient = "Tăng số lượng thành công",
                    mesDev = "Plus quantity success",
                    priceItem = string.Format(cultureInfo, "{0:C0}", priceItem),
                    amountItem = string.Format(cultureInfo, "{0:C0}", priceItem * cartItem.Quantity),
                    quantityItem = cartItem.Quantity,
                    amount = string.Format(cultureInfo, "{0:C0}", amount),
                    sumItem = cart.Sum(c => c.Quantity)
                });
            }
            catch 
            {
                return BadRequest(new { mesClient = "Thêm số lượng thất bại", mesDev = "Plus quantity is failse" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> MinusCart(string productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Giảm số lượng thất bại do không tìm thấy tài khoản đăng nhập", 
                        mesDev = "Account not found" 
                    });
                }
                var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                if (customer == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Giảm số lượng thất bại do không tìm thấy khách hàng đang đăng nhập", 
                        mesDev = "Customer not found" 
                    });
                }
                var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Giảm số lượng thất bại do không tìm thấy sản phẩm cần thêm", 
                        mesDev = "Product not found" 
                    });
                }
                var cartItem = await _cartService.GetSingleByConditionAsync(c => c.CustomerId == customer.CustomerId && c.ProductId == productId);
                if (cartItem == null)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Giảm số lượng thất bại do không tìm thấy giỏ hàng", 
                        mesDev = "Cart not found" 
                    });
                }
                if (cartItem.Quantity <= 1)
                {
                    return BadRequest(new 
                    { 
                        mesClient = "Giảm số lượng thất bại do số lượng sẽ nhỏ hơn 1", 
                        mesDev = "quantity in cart is bigger than product's quantity" 
                    });
                }
                cartItem.Quantity = cartItem.Quantity - 1;
                await _cartService.UpdateAsync(cartItem);
                var priceItem = (product.PercentDiscount == null || product.PercentDiscount == 0) ? product.Price
                                    : product.Price - (product.Price * (decimal)product.PercentDiscount / 100);
                var cart = await _cartService.Table()
                                            .Include(c => c.Product)
                                            .Where(c => c.CustomerId == customer.CustomerId)
                                            .ToListAsync();
                var amount = cart.Sum(c =>
                {
                    var price = (c.Product.PercentDiscount == null || c.Product.PercentDiscount == 0) ? c.Product.Price
                                    : c.Product.Price - (c.Product.Price * (decimal)c.Product.PercentDiscount / 100);
                    return c.Quantity * price;
                });
                return Ok(new
                {
                    mesClient = "Giảm số lượng thành công",
                    mesDev = "Minus quantity success",
                    priceItem = string.Format(cultureInfo, "{0:C0}", priceItem),
                    amountItem = string.Format(cultureInfo, "{0:C0}", priceItem * cartItem.Quantity),
                    quantityItem = cartItem.Quantity,
                    amount = string.Format(cultureInfo, "{0:C0}", amount),
                    sumItem = cart.Sum(c => c.Quantity)
                });
            }
            catch
            {
                return BadRequest(new 
                { 
                    mesClient = "Giảm số lượng thất bại", 
                    mesDev = "Minus quantity is false" 
                });
            }
        }
    }
}
