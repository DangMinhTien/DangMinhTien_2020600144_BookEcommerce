using Book_Ecommerce.Data;
using Book_Ecommerce.Helpers;
using Book_Ecommerce.Models.Cart;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private List<CartItem> _cart
        {
            get
            {
                return HttpContext.Session.Get<List<CartItem>>(MyAppSetting.CART_KEY) ?? new List<CartItem>();
            }
        }
        public IViewComponentResult Invoke()
        {
            return View(_cart.Sum(c => c.Quantity));
        }
    }
}
