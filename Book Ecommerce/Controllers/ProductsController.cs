using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;


namespace Book_Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IAuthorService _authorService;

        public ProductsController(AppDbContext context, 
            IProductService productService,
            ICategoryService categoryService,
            IBrandService brandService,
            IAuthorService authorService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _authorService = authorService;
        }
        [Route("sanpham.html")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    ViewBag.search = search;
                }
                (var products, var pagingModel, var lstPageSize) = 
                    await _productService.GetToViewAsync(search, page, pagesize);
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSize = lstPageSize;
                return View(products);
            }
            catch(Exception ex)
            {
                return NotFound("Không thể lấy được sản phẩm");
            }
        }
        [Route("/sanpham/theloai/{categorySlug}")]
        public async Task<IActionResult> GetByCategory(string categorySlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                var category = await _categoryService.GetSingleByConditionAsync(c => c.CategorySlug == categorySlug);
                if (category == null)
                {
                    return NotFound("không tìm thấy thể loại cần xem");
                }
                (var products, var pagingModel, var lstPageSize) = 
                    await _productService.GetByCategoryToViewAsync(category.CategoryId, categorySlug, page, pagesize);
            
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSize = lstPageSize;
                ViewBag.categoryId = category.CategoryId;
                return View("Index", products);

            }
            catch
            {
                return NotFound("Không thể lấy được sản phẩm");
            }
        }
        [Route("/sanpham/thuonghieu/{brandSlug}")]
        public async Task<IActionResult> GetByBrand(string brandSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                var brand = await _brandService.GetSingleByConditionAsync(b => b.BrandSlug == brandSlug);
                if (brand == null)
                {
                    return NotFound("không tìm thấy thương hiệu cần xem");
                }
                (var products, var pagingModel, var lstPageSize) = 
                    await _productService.GetByBrandToViewAsync(brand.BrandId, brandSlug, page, pagesize);
            
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSize = lstPageSize;
                ViewBag.brandId = brand.BrandId;
                return View("Index", products);

            }
            catch
            {
                return NotFound("Không thể lấy được sản phẩm");
            }
        }
        [Route("/sanpham/tacgia/{authorSlug}")]
        public async Task<IActionResult> GetByAuthor(string authorSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            try
            {
                var author = await _authorService.GetSingleByConditionAsync(c => c.AuthorSlug == authorSlug);
                if (author == null)
                {
                    return NotFound("không tìm thấy tác giả cần xem");
                }
                (var products, var pagingModel, var lstPageSize) = 
                    await _productService.GetByAuthorToViewAsync(author.AuthorId, authorSlug, page, pagesize);
            
                ViewBag.pagingModel = pagingModel;
                ViewBag.pageSize = lstPageSize;
                ViewBag.authorId = author.AuthorId;
                return View("Index", products);
            }
            catch
            {
                return NotFound("Không thể lấy được sản phẩm");
            }
        }
        [Route("/sanpham/{productSlug}.html")]
        public async Task<IActionResult> Detail(string productSlug)
        {
            var product = await _productService.GetDetailToViewAsync(productSlug);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }
            return View(product);
        }
    }
}
