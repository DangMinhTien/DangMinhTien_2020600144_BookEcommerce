using Book_Ecommerce.Domain.ViewModels.BannerViewModel;
using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.ViewComponents
{
    public class SliderBannerViewComponent : ViewComponent
    {
        private readonly IBannerService _bannerService;

        public SliderBannerViewComponent(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var banners = await _bannerService.GetDataAsync();
            var result = banners.Select(b => new BannerVM
            {
               BannerId = b.BannerId,
               BannerCode = b.BannerCode,
               Title = b.Title,
               Content = b.Content,
               UrlImage = b.UrlImage,
            }).ToList();
            return View(result);
        }
    }
}
