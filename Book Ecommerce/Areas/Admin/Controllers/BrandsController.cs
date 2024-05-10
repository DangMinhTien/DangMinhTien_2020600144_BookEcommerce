using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.AuthorViewModel;
using Book_Ecommerce.Domain.ViewModels.BrandViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<BrandsController> _logger;

        public BrandsController(IBrandService brandService, 
            ICloudinaryService cloudinaryService,
            ILogger<BrandsController> logger)
        {
            _brandService = brandService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }
        [HttpGet("/quan-ly-thuong-hieu")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                ViewBag.search = search;
            }
            (var brandVMs, var pagingModel, var lstPageSize) = await _brandService.GetToViewManageAsync(search, page, pagesize);
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            ViewBag.pagesize = pagesize;
            return View(brandVMs);
        }
        [HttpPost("/quan-ly-thương-hieu/themmoi")]
        public async Task<IActionResult> Create(InputBrand inputBrand)
        {
            if(inputBrand.FileImage == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn phải chọn ảnh cho thuong hiệu");
            }
            if (_brandService.Table().Any(b => b.BrandName == inputBrand.BrandName))
            {
                ModelState.AddModelError(string.Empty, "Tên thương hiệu bị trùng với thương hiệu khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (inputBrand.FileImage == null)
                    {
                        return BadRequest(new 
                        { 
                            error = new List<string> { "Bạn phải chọn ảnh cho thương hiệu"}, 
                            isValid = false, 
                            mesClient = "Lỗi nhập dữ liệu", 
                            mesDev = "error input data" 
                        });
                    }
                    var cloudinaryModel = await _cloudinaryService.UploadAsync(inputBrand.FileImage);
                    var codeNumber = _brandService.Table().Count() > 0 ?
                        _brandService.Table().Max(b => b.CodeNumber) + 1 : 1000;
                    var brandSlug = string.Empty;
                    do
                    {
                        brandSlug = Generation.GenerationSlug(inputBrand.BrandName);
                    } while (_brandService.Table().Any(b => b.BrandSlug == brandSlug));
                    var brand = new Brand
                    {
                        BrandId = Guid.NewGuid().ToString(),
                        CodeNumber = codeNumber,
                        BrandCode = "TH" + DateTime.Now.Year.ToString() + codeNumber,
                        BrandName = inputBrand.BrandName,
                        BrandSlug = brandSlug,
                        Description = inputBrand.Description,
                        ImageName = cloudinaryModel.FileName,
                        UrlImage = cloudinaryModel.Url
                    };
                    await _brandService.AddAsync(brand);
                    TempData["success"] = "Thêm thương hiệu thành công";
                    return Ok(new { mesClient = "Thêm thương hiệu thành công", mesDev = "add brand is successfully" });
                }
                catch (Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi thêm thương hiệu", mesDev = ex.Message });
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
        [HttpGet("/quan-ly-thuong-hieu/chitiet")]
        public async Task<IActionResult> Detail(string brandId)
        {
            try
            {
                var brand = await _brandService.GetSingleByConditionAsync(b => b.BrandId == brandId);
                if (brand == null)
                {
                    return BadRequest(new { mesClient = "Không cập nhật được do thông tìm thấy thương hiệu", mesDev = "Brand is not found" });
                }
                return Ok(new
                {
                    brandName = brand.BrandName,
                    description = brand.Description,
                    urlImage = brand.UrlImage,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được thương hiệu cần sửa", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-thuong-hieu/capnhat")]
        public async Task<IActionResult> Update(string brandId, InputBrand inputBrand)
        {
            if (_brandService.Table().Any(b => b.BrandName == inputBrand.BrandName && b.BrandId != brandId))
            {
                ModelState.AddModelError(string.Empty, "Tên tác giả bị trùng với tác giả khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var brand = await _brandService.GetSingleByConditionAsync(b => b.BrandId == brandId);
                    if (brand == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không cập nhật được do thông tìm thấy thương hiệu", mesDev = "Brand is not found" });
                    }
                    var oldImage = string.Empty;
                    if(inputBrand.FileImage != null)
                    {
                        var cloudinaryModel = await _cloudinaryService.UploadAsync(inputBrand.FileImage);
                        oldImage = brand.ImageName;
                        brand.ImageName = cloudinaryModel.FileName;
                        brand.UrlImage = cloudinaryModel.Url;
                    }
                    var brandSlug = string.Empty;
                    do
                    {
                        brandSlug = Generation.GenerationSlug(inputBrand.BrandName);
                    } while (_brandService.Table().Any(b => b.BrandSlug == brandSlug && b.BrandId != brandId));
                    brand.BrandName = inputBrand.BrandName;
                    brand.Description = inputBrand.Description;
                    brand.BrandSlug = brandSlug;
                    await _brandService.UpdateAsync(brand);
                    if (inputBrand.FileImage != null && !string.IsNullOrEmpty(oldImage))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteAsync(oldImage);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogInformation($"Có lỗi cloudinary: {ex.Message}");
                        }
                    }
                    TempData["success"] = "Cập nhật tác giả thành công";
                    return Ok(new { mesClient = "Cập nhật tác giả thành công", mesDev = "update author is successfully" });
                }
                catch (Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi cập nhật tác giả", mesDev = ex.Message });
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
        [HttpPost("/quan-ly-thuong-hieu/xoa")]
        public async Task<IActionResult> Delete(string brandId)
        {
            try
            {
                var brand = await _brandService.GetSingleByConditionAsync(b => b.BrandId == brandId);
                if (brand == null)
                {
                    return BadRequest(new { mesClient = "Không xóa được do thông tìm thấy thương hiệu", mesDev = "Brand is not found" });
                }
                await _brandService.RemoveAsync(brand);
                try
                {
                    await _cloudinaryService.DeleteAsync(brand.ImageName);
                }
                catch(Exception ex)
                {
                    _logger.LogInformation($"Có lỗi cloudinary: {ex.Message}");
                }
                TempData["success"] = "Xóa thương hiệu thành công";
                return Ok(new { mesClient = "Xóa thương hiệu thành công", mesDev = "delete brand is successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không xóa được thương hiệu", mesDev = ex.Message });
            }
        }
    }
}
