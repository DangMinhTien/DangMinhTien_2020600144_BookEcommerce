using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarCategoryViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SideBarCategoryViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? CategotyId)
        {
            var categories = await _context.Categories.Include(c => c.CategoryProducts).ToListAsync();
            var result = categories.Select(c => new CategoryVM
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryCode = c.CategoryCode,
                CategorySlug = c.CategorySlug,
                IsActive = c.CategoryId == CategotyId,
                SumProduct = c.CategoryProducts == null ? 0 : c.CategoryProducts.Count(),
            }).ToList();
            return View(result);
        }
    }
}
