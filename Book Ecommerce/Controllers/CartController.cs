using Book_Ecommerce.Data;
using Book_Ecommerce.Helpers;
using Book_Ecommerce.Models;
using Book_Ecommerce.Models.Cart;
using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Book_Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private List<CartItem> _cart {
            get
            {
                return HttpContext.Session.Get<List<CartItem>>(MyAppSetting.CART_KEY) ?? new List<CartItem>();
            }
        }
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if(_cart.Count == 0)
            {
                TempData["infor"] = "Giỏ hàng hiện đang trống";
                return RedirectToAction("Index", "Home");
            }
            var myCart = _cart;
            var myCartVM = new List<CartVM>();
            for(int i = 0; i < myCart.Count(); i++)
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == myCart[i].ProductId);
                if(product == null)
                {
                    myCart.RemoveAt(i);
                    HttpContext.Session.Set<List<CartItem>>(MyAppSetting.CART_KEY, myCart);
                    i--;
                    continue;
                }
                myCartVM.Add(new CartVM
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductSlug = product.ProductSlug,
                    Quantity = myCart[i].Quantity,
                    Price = (product.PercentDiscount > 0) ? product.Price - (product.Price * ((decimal)product.PercentDiscount / 100)) : product.Price,
                    Image = product.Images.FirstOrDefault()?.ImageName ?? ""
                });
            }
            if (_cart.Count == 0)
            {
                TempData["infor"] = "Giỏ hàng hiện đang trống";
                return RedirectToAction("Index", "Home");
            }
            return View(myCartVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1)
        {
            var myCart = _cart;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null) { return BadRequest(new {message = "Thêm vào giỏ hàng thất bại vì không tìm thấy sản phẩm cần thêm vào giỏ hàng."}); }
            var cartItem = myCart.FirstOrDefault(p => p.ProductId == productId);
            if (quantity < 1)
                return Json(new
                {
                    state = false,
                    messageclient = "Thêm vào giỏ hàng thất bại vì số lượng thêm vào nhỏ hơn 1.",
                    messagedev = "\r\nAdd to cart failed because the quantity added was less than 1."
                });
            if (cartItem == null)
            {
                if(quantity > product.Quantity)
                    return Json(new
                    {
                        state = false,
                        messageclient = "Thêm vào giỏ hàng thất bại vì số lượng thêm vào lớn hơn số lượng sản phẩm.",
                        messagedev = "Add to cart failed because the quantity added was greater than the product quantity."
                    });
                myCart.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                if(cartItem.Quantity + quantity < 1)
                    return Json(new
                    {
                        state = false,
                        messageclient = "Thêm vào giỏ hàng thất bại vì sẽ làm số lượng trong giỏ nhỏ hơn 1.",
                        messagedev = "\r\nAdding to cart fails because it will make the quantity in the cart smaller than 1."
                    });
                if(cartItem.Quantity  + quantity > product.Quantity)
                    return Json(new
                    {
                        state = false,
                        messageclient = "Thêm vào giỏ hàng thất bại vì số lượng trong giỏ sẽ lớn hơn số lượng sản phẩm.",
                        messagedev = "Add to cart fails because the quantity in the cart will be greater than the quantity of products."
                    });
                cartItem.Quantity = cartItem.Quantity + quantity;
            }
            HttpContext.Session.Set<List<CartItem>>(MyAppSetting.CART_KEY, myCart);
            return Json(new
            {
                state = true,
                messageclient = "Thêm vào giỏ hàng thành công.",
                messagedev = "\r\nAdd to cart successfully.",
                sumitem = _cart.Sum(c => c.Quantity)
            });
        }
        [HttpPost]
        public async Task<IActionResult> AlterTheCart(string productId, int quantity = 1)
        {
            CultureInfo cultureInfo = new CultureInfo("vi-VN");
            var myCart = _cart;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null) { return BadRequest(new { message = "Thay đổi số lượng trong giỏ hàng thất bại vì không tìm thấy sản phẩm cần thêm vào giỏ hàng." }); }
            var cartItem = myCart.FirstOrDefault(p => p.ProductId == productId);
            if (cartItem == null)
            {
                return BadRequest(new { message = "Thay đổi số lượng trong giỏ hàng thất bại vì không tìm thấy sản phẩm trong giỏ hàng." });
            }
            else
            {
                if (cartItem.Quantity + quantity < 1)
                    return Json(new
                    {
                        state = false,
                        messageclient = "Thay đổi số lượng trong giỏ hàng thất bại vì sẽ làm số lượng trong giỏ nhỏ hơn 1.",
                        messagedev = "\r\nAlter the cart fails because it will make the quantity in the cart smaller than 1."
                    });
                if (cartItem.Quantity + quantity > product.Quantity)
                    return Json(new
                    {
                        state = false,
                        messageclient = "Thay đổi số lượng trong giỏ hàng thất bại vì số lượng trong giỏ sẽ lớn hơn số lượng sản phẩm.",
                        messagedev = "Alter the cart fails because the quantity in the cart will be greater than the quantity of products."
                    });
                cartItem.Quantity = cartItem.Quantity + quantity;
            }
            HttpContext.Session.Set<List<CartItem>>(MyAppSetting.CART_KEY, myCart);
            var priceItem = (product.PercentDiscount > 0) ? product.Price - (product.Price * ((decimal)product.PercentDiscount / 100)) : product.Price;
            return Json(new
            {
                state = true,
                messageclient = "Thay đổi số lượng trong giỏ hàng thành công.",
                messagedev = "\r\nAlter the cart successfully.",
                sumitem = _cart.Sum(c => c.Quantity),
                amount = string.Format(cultureInfo, "{0:C0}", await GetAmountCart()),
                total = string.Format(cultureInfo, "{0:C0}", await GetAmountCart() + MyAppSetting.TRANSPORT_FEE),
                transportfee = MyAppSetting.TRANSPORT_FEE,
                priceitem = string.Format(cultureInfo, "{0:C0}", priceItem),
                amountitem = string.Format(cultureInfo, "{0:C0}", priceItem * cartItem.Quantity),
                quantityitem = cartItem.Quantity,
            });
        }
        public IActionResult RemoveCartItem(string productid)
        {
            var myCart = _cart;
            var cartItem = myCart.FirstOrDefault(c => c.ProductId == productid);
            if (cartItem == null)
                return NotFound();
            myCart.Remove(cartItem);
            HttpContext.Session.Set<List<CartItem>>(MyAppSetting.CART_KEY, myCart);
            TempData["success"] = "Xóa một sản phẩm trong giỏ hàng thành công";
            return RedirectToAction("Index");
        }
        public async Task<decimal> GetAmountCart()
        {
            if (_cart.Count == 0)
            {
                return 0;
            }
            var myCart = _cart;
            var myCartVM = new List<CartVM>();
            for (int i = 0; i < myCart.Count(); i++)
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == myCart[i].ProductId);
                if (product == null)
                {
                    myCart.RemoveAt(i);
                    HttpContext.Session.Set<List<CartItem>>(MyAppSetting.CART_KEY, myCart);
                    i--;
                    continue;
                }
                myCartVM.Add(new CartVM
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductSlug = product.ProductSlug,
                    Quantity = myCart[i].Quantity,
                    Price = (product.PercentDiscount > 0) ? product.Price - (product.Price * ((decimal)product.PercentDiscount / 100)) : product.Price,
                    Image = product.Images.FirstOrDefault()?.ImageName ?? ""
                });
            }
            if (_cart.Count == 0)
            {
                return 0;
            }
            return myCartVM.Sum(c => c.Amount);
        }
        public IActionResult Test()
        {
            var test = _cart;
            test.Add(new CartItem
            {
                ProductId = "aaaaaaaaa",
                Quantity = 1,
            });
            HttpContext.Session.Set<List<CartItem>>(MyAppSetting.CART_KEY, test);
            return RedirectToAction("Index", "Home");
        }
    }
}
