using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.AuthorViewModel;
using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorService authorService,
            ICloudinaryService cloudinaryService,
            ILogger<AuthorsController> logger)
        {
            _authorService = authorService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }
        [HttpGet("/quan-ly-tac-gia")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.search = search;
            }
            (var authorVMs, var pagingModel, var lstPageSize) = await _authorService.GetToViewManageAsync(search, page, pagesize);
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            ViewBag.pagesize = pagesize;
            return View(authorVMs);
        }
        [HttpPost("/quan-ly-tac-gia/themmoi")]
        public async Task<IActionResult> Create(InputAuthor inputAuthor)
        {
            if (_authorService.Table().Any(a => a.AuthorName == inputAuthor.AuthorName))
            {
                ModelState.AddModelError(string.Empty, "Tên tác giả bị trùng với tác giả khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var codeNumber = _authorService.Table().Count() > 0 ?
                        _authorService.Table().Max(a => a.CodeNumber) + 1 : 1000;
                    var authorSlug = string.Empty;
                    do
                    {
                        authorSlug = Generation.GenerationSlug(inputAuthor.AuthorName);
                    } while (_authorService.Table().Any(a => a.AuthorSlug == authorSlug));
                    var author = new Author
                    {
                        AuthorId = Guid.NewGuid().ToString(),
                        CodeNumber = codeNumber,
                        AuthorCode = "TG" + DateTime.Now.Year.ToString() + codeNumber,
                        AuthorName = inputAuthor.AuthorName,
                        AuthorSlug = authorSlug,
                        Information = inputAuthor.Information,
                    };
                    if(inputAuthor.FileImage != null)
                    {
                        var clodinaryModel = await _cloudinaryService.UploadAsync(inputAuthor.FileImage);
                        author.UrlImage = clodinaryModel.Url;
                        author.FileImage = clodinaryModel.FileName;
                    }
                    else
                    {
                        author.UrlImage = null;
                        author.FileImage = null;
                    }
                    await _authorService.AddAsync(author);
                    TempData["success"] = "Thêm tác giả thành công";
                    return Ok(new { mesClient = "Thêm tác giả thành công", mesDev = "add author is successfully" });
                }
                catch (Exception ex)
                {
                    BadRequest(new { isValid = true, mesClient = "Lỗi khi thêm tác giả", mesDev = ex.Message });
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
        [HttpGet("/quan-ly-tac-gia/chitiet")]
        public async Task<IActionResult> Detail(string authorId)
        {
            try
            {
                var author = await _authorService.GetSingleByConditionAsync(a => a.AuthorId == authorId);
                if (author == null)
                {
                    return BadRequest(new { mesClient = "Không cập nhật được do thông tìm thấy tác giả", mesDev = "Author is not found" });
                }
                return Ok(new
                {
                    authorName = author.AuthorName,
                    information = author.Information,
                    urlImage = author.UrlImage,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được tác giả cần sửa", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-tac-gia/capnhat")]
        public async Task<IActionResult> Update(string authorId, InputAuthor inputAuthor)
        {
            if (_authorService.Table().Any(a => a.AuthorName == inputAuthor.AuthorName && a.AuthorId != authorId))
            {
                ModelState.AddModelError(string.Empty, "Tên tác giả bị trùng với tác giả khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var author = await _authorService.GetSingleByConditionAsync(a => a.AuthorId == authorId);
                    if (author == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không cập nhật được do thông tìm thấy tác giả", mesDev = "Author is not found" });
                    }
                    var oldFileImage = string.Empty;
                    if(inputAuthor.FileImage != null)
                    {
                        var clodinaryModel = await _cloudinaryService.UploadAsync(inputAuthor.FileImage);
                        oldFileImage = author.FileImage;
                        author.UrlImage = clodinaryModel.Url;
                        author.FileImage = clodinaryModel.FileName;
                    }
                    var authorSlug = string.Empty;
                    do
                    {
                        authorSlug = Generation.GenerationSlug(inputAuthor.AuthorName);
                    } while (_authorService.Table().Any(a => a.AuthorSlug == authorSlug && a.AuthorId != authorId));
                    author.AuthorName = inputAuthor.AuthorName;
                    author.Information = inputAuthor.Information;
                    author.AuthorSlug = authorSlug;
                    await _authorService.UpdateAsync(author);
                    if (inputAuthor.FileImage != null)
                    {
                        try
                        {
                            if(!string.IsNullOrEmpty(oldFileImage))
                            {
                                await _cloudinaryService.DeleteAsync(oldFileImage);
                            }
                        }
                        catch(Exception ex)
                        {
                            _logger.LogInformation("Có lỗi cloudinay:" + ex.Message);
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
        [HttpPost("/quan-ly-tac-gia/xoa")]
        public async Task<IActionResult> Delete(string authorId)
        {
            try
            {
                var author = await _authorService.GetSingleByConditionAsync(a => a.AuthorId == authorId);
                if (author == null)
                {
                    return BadRequest(new { mesClient = "Không xóa được do thông tìm thấy tác giả", mesDev = "Author is not found" });
                }
                await _authorService.RemoveAsync(author);
                if (!string.IsNullOrEmpty(author.FileImage))
                {
                    try
                    {
                        await _cloudinaryService.DeleteAsync(author.FileImage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Có lỗi cloudinay:" + ex.Message);
                    }
                }
                TempData["success"] = "Xóa tác giả thành công";
                return Ok(new { mesClient = "Xóa tác giả thành công", mesDev = "delete category is successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không xóa được tác giả", mesDev = ex.Message });
            }
        }
    }
}
