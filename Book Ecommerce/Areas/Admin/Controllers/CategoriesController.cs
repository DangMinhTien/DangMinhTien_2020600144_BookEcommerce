using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) 
        {
            _categoryService = categoryService;
        }
        [HttpGet("/quan-ly-the-loai")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.search = search;
            }
            (var categoryVMs, var pagingModel, var lstPageSize) = await _categoryService.GetToViewManageAsync(search, page, pagesize);
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            ViewBag.pagesize = pagesize;
            return View(categoryVMs);
        }
        [HttpPost("/quan-ly-the-loai/themmoi")]
        public async Task<IActionResult> Create(InputCategory inputCategory)
        {
            if(_categoryService.Table().Any(c => c.CategoryName == inputCategory.CategoryName))
            {
                ModelState.AddModelError(string.Empty, "Tên thể loại bị trùng với thể loại khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var codeNumber = _categoryService.Table().Count() > 0 ?
                        _categoryService.Table().Max(c => c.CodeNumber) + 1 : 1000;
                    var categorySlug = string.Empty;
                    do
                    {
                        categorySlug = Generation.GenerationSlug(inputCategory.CategoryName);
                    }while(_categoryService.Table().Any(c => c.CategorySlug == categorySlug));
                    var category = new Category
                    {
                        CategoryId = Guid.NewGuid().ToString(),
                        CodeNumber = codeNumber,
                        CategoryCode = "TL" + DateTime.Now.Year.ToString() + codeNumber,
                        CategoryName = inputCategory.CategoryName,
                        CategorySlug = categorySlug,
                        Description = inputCategory.Description,
                    };
                    await _categoryService.AddAsync(category);
                    TempData["success"] = "Thêm thể loại thành công";
                    return Ok(new {mesClient = "Thêm thể loại thành công", mesDev = "add category is successfully"});
                }
                catch(Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi thêm thể loại", mesDev = ex.Message });
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
        [HttpGet("/quan-ly-the-loai/chitiet")]
        public async Task<IActionResult> Detail(string categoryId)
        {
            try
            {
                var category = await _categoryService.GetSingleByConditionAsync(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    return BadRequest(new { mesClient = "Không cập nhật được do thông tìm thấy thể loại", mesDev = "Category is not found" });
                }
                return Ok(new
                {
                    categoryName = category.CategoryName,
                    description = category.Description,
                });
            }catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được thể loại cần sửa", mesDev = ex.Message });
            }

        }
        [HttpPost("/quan-ly-the-loai/capnhat")]
        public async Task<IActionResult> Update(string categoryId, InputCategory inputCategory)
        {
            if (_categoryService.Table().Any(c => c.CategoryName == inputCategory.CategoryName && c.CategoryId != categoryId))
            {
                ModelState.AddModelError(string.Empty, "Tên thể loại bị trùng với thể loại khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _categoryService.GetSingleByConditionAsync(c => c.CategoryId == categoryId);
                    if(category == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không cập nhật được do thông tìm thấy thể loại", mesDev = "Category is not found" });
                    }
                    var categorySlug = string.Empty;
                    do
                    {
                        categorySlug = Generation.GenerationSlug(inputCategory.CategoryName);
                    } while (_categoryService.Table().Any(c => c.CategorySlug == categorySlug && c.CategoryId != categoryId));
                    category.CategoryName = inputCategory.CategoryName;
                    category.Description = inputCategory.Description;
                    category.CategorySlug = categorySlug;
                    await _categoryService.UpdateAsync(category);
                    TempData["success"] = "Cập nhật thể loại thành công";
                    return Ok(new { mesClient = "Cập nhật thể loại thành công", mesDev = "update category is successfully" });
                }
                catch (Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi cập nhật thể loại", mesDev = ex.Message });
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
        [HttpPost("/quan-ly-the-loai/xoa")]
        public async Task<IActionResult> Delete(string categoryId)
        {
            try
            {
                var category = await _categoryService.GetSingleByConditionAsync(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    return BadRequest(new { mesClient = "Không xóa được do thông tìm thấy thể loại", mesDev = "Category is not found" });
                }
                await _categoryService.RemoveAsync(category);
                TempData["success"] = "Xóa thể loại thành công";
                return Ok(new { mesClient = "Xóa thể loại thành công", mesDev = "delete category is successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không xóa được thể loại", mesDev = ex.Message });
            }
        }
    }
}
