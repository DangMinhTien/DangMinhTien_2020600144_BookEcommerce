using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public SideBarCategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? CategotyId)
        {
            var categories = await _categoryService.GetToViewComponentAsync();
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
