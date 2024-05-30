using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.BannerViewModel;
using Book_Ecommerce.Domain.ViewModels.BrandViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class BannersController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<BrandsController> _logger;

        public BannersController(IBannerService bannerService,
            ICloudinaryService cloudinaryService,
            ILogger<BrandsController> logger)
        {
            _bannerService = bannerService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }
        [HttpGet("/quan-ly-banner")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                ViewBag.search = search;
            }
            (var bannerVMs, var pagingModel, var lstPageSize) = await _bannerService.GetToViewManageAsync(search, page, pagesize);
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            ViewBag.pagesize = pagesize;
            return View(bannerVMs);
        }
        [HttpPost("/quan-ly-banner/themmoi")]
        public async Task<IActionResult> Create(InputBanner inputBanner)
        {
            if (inputBanner.FileImage == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn phải chọn ảnh cho banner");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (inputBanner.FileImage == null)
                    {
                        return BadRequest(new
                        {
                            error = new List<string> { "Bạn phải chọn ảnh cho banner" },
                            isValid = false,
                            mesClient = "Lỗi nhập dữ liệu",
                            mesDev = "error input data"
                        });
                    }
                    var cloudinaryModel = await _cloudinaryService.UploadAsync(inputBanner.FileImage);
                    var codeNumber = _bannerService.Table().Count() > 0 ?
                        _bannerService.Table().Max(b => b.CodeNumber) + 1 : 1000;
                    var banner = new Banner
                    {
                        BannerId = Guid.NewGuid().ToString(),
                        Title = inputBanner.Title,
                        Content = inputBanner.Content,
                        CodeNumber = codeNumber,
                        BannerCode = "BN" + DateTime.Now.Year.ToString() + codeNumber,
                        ImageName = cloudinaryModel.FileName,
                        UrlImage = cloudinaryModel.Url
                    };
                    await _bannerService.AddAsync(banner);
                    TempData["success"] = "Thêm banner thành công";
                    return Ok(new { mesClient = "Thêm banner thành công", mesDev = "add banner is successfully" });
                }
                catch (Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi thêm banner", mesDev = ex.Message });
                }
            }
            List<string> error = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var err in value.Errors)
                {
                    error.Add(err.ErrorMessage);
                }
            }
            return BadRequest(new { error = error, isValid = false, mesClient = "Lỗi nhập dữ liệu", mesDev = "error input data" });
        }
        [HttpGet("/quan-ly-banner/chitiet")]
        public async Task<IActionResult> Detail(string bannerId)
        {
            try
            {
                var banner = await _bannerService.GetSingleByConditionAsync(b => b.BannerId == bannerId);
                if (banner == null)
                {
                    return BadRequest(new { mesClient = "Không cập nhật được do thông tìm thấy banner", mesDev = "Banner is not found" });
                }
                return Ok(new
                {
                    title = banner.Title,
                    content = banner.Content,
                    urlImage = banner.UrlImage,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được banner cần sửa", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-banner/capnhat")]
        public async Task<IActionResult> Update(string bannerId, InputBanner inputBanner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var banner = await _bannerService.GetSingleByConditionAsync(b => b.BannerId == bannerId);
                    if (banner == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không cập nhật được do thông tìm thấy banner", mesDev = "Banner is not found" });
                    }
                    var oldImage = string.Empty;
                    if (inputBanner.FileImage != null)
                    {
                        var cloudinaryModel = await _cloudinaryService.UploadAsync(inputBanner.FileImage);
                        oldImage = banner.ImageName;
                        banner.ImageName = cloudinaryModel.FileName;
                        banner.UrlImage = cloudinaryModel.Url;
                    }
                    banner.Title = inputBanner.Title;
                    banner.Content = inputBanner.Content;
                    await _bannerService.UpdateAsync(banner);
                    if (inputBanner.FileImage != null && !string.IsNullOrEmpty(oldImage))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteAsync(oldImage);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation($"Có lỗi cloudinary: {ex.Message}");
                        }
                    }
                    TempData["success"] = "Cập nhật banner thành công";
                    return Ok(new { mesClient = "Cập nhật banner thành công", mesDev = "update banner is successfully" });
                }
                catch (Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi cập nhật banner", mesDev = ex.Message });
                }
            }
            List<string> error = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var err in value.Errors)
                {
                    error.Add(err.ErrorMessage);
                }
            }
            return BadRequest(new { error = error, isValid = false, mesClient = "Lỗi nhập dữ liệu", mesDev = "error input data" });
        }
        [HttpPost("/quan-ly-banner/xoa")]
        public async Task<IActionResult> Delete(string bannerId)
        {
            try
            {
                var banner = await _bannerService.GetSingleByConditionAsync(b => b.BannerId == bannerId);
                if (banner == null)
                {
                    return BadRequest(new { mesClient = "Không xóa được do thông tìm thấy banner", mesDev = "Banner is not found" });
                }
                await _bannerService.RemoveAsync(banner);
                try
                {
                    await _cloudinaryService.DeleteAsync(banner.ImageName);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Có lỗi cloudinary: {ex.Message}");
                }
                TempData["success"] = "Xóa banner thành công";
                return Ok(new { mesClient = "Xóa banner thành công", mesDev = "delete banner is successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không xóa được banner", mesDev = ex.Message });
            }
        }
    }
}
