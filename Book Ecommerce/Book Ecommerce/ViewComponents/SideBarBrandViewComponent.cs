using Book_Ecommerce.Domain.ViewModels.BrandViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;

namespace Book_Ecommerce.ViewComponents
{
    public class SideBarBrandViewComponent : ViewComponent
    {
        private readonly IBrandService _brandService;

        public SideBarBrandViewComponent(IBrandService brandService)
        {
            _brandService = brandService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? BrandId)
        {
            var brands = await _brandService.GetToViewComponentAsync();
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
