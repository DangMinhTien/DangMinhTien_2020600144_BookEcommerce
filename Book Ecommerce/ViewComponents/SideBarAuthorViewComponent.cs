using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarAuthorViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SideBarAuthorViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? AuthorId)
        {
            var authors = await _context.Authors.Include(b => b.AuthorProducts)
                                                .ThenInclude(ap => ap.Product)            
                                                .ToListAsync();
            var result = authors.Select(a => new AuthorVM
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                AuthorCode = a.AuthorCode,
                AuthorSlug = a.AuthorSlug,
                SumProduct = a.AuthorProducts != null ? a.AuthorProducts.Count() : 0,
                IsActive = a.AuthorId == AuthorId ? true : false,
                Information = a.Information
            }).ToList();
            return View(result);
        }
    }
}
