using Book_Ecommerce.Models;
using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarBrandViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SideBarBrandViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? BrandId)
        {
            var brands = await _context.Brands.Include(b => b.Products).ToListAsync();
            var result = brands.Select(b => new BrandVM
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                BrandSlug = b.BrandSlug,
                BrandCode = b.BrandCode,
                IsActive = b.BrandId == BrandId,
                SumProduct = b.Products == null ? 0 : b.Products.Count()
            }).ToList();
            return View(result);
        }
    }
}
