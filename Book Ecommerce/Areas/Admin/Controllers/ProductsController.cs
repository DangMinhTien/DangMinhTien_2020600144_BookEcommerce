using Book_Ecommerce.Data;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IBrandService _brandService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(AppDbContext context,
            IProductService productService,
            ICategoryService categoryService,
            IAuthorService authorService,
            IBrandService brandService,
            ICloudinaryService cloudinaryService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _authorService = authorService;
            _brandService = brandService;
            _cloudinaryService = cloudinaryService;
        }
        [HttpGet("/quan-ly-san-pham.html")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.search = search;
            }
            (var productVMs, var pagingModel, var lstPageSize) = await _productService.GetToViewAdminAsync(search, page, pagesize);
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            var returnUrl = Url.Action("Index", "Products", new 
            { 
                area = "Admin", 
                page = page, 
                search = search, 
                pagesize = pagesize 
            });
            ViewBag.returnUrl = returnUrl;
            return View(productVMs);
        }
        [HttpGet("/quan-ly-san-pham/them-moi.html")]
        public async Task<IActionResult> Create(string? returnUrl)
        {
            var authors = await _authorService.GetDataAsync();
            var categories = await _categoryService.GetDataAsync();
            var brands = await _brandService.GetDataAsync();
            ViewBag.authors = new MultiSelectList(authors, "AuthorId", "AuthorName");
            ViewBag.categories = new MultiSelectList(categories, "CategoryId", "CategoryName");
            ViewBag.brands = new SelectList(brands, "BrandId", "BrandName");
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost("/quan-ly-san-pham/them-moi.html")]
        public async Task<IActionResult> Create(CreateProduct createProduct)
        {
            if(_productService.Table().Any(p => p.ProductName == createProduct.ProductName))
            {
                ModelState.AddModelError(string.Empty, "Tên sản phẩm vừa nhập bị trùng với một sản phẩm khác");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string productSlug = string.Empty;
                    do
                    {
                        productSlug = Generation.GenerationSlug(createProduct.ProductName);
                    }while(_productService.Table().Any(p => p.ProductSlug == productSlug));
                    var productId = Guid.NewGuid().ToString();
                    List<Image> images = new List<Image>();
                    foreach(var file in createProduct.ImageFiles)
                    {
                        var cloudinaryModel = await _cloudinaryService.UploadAsync(file);
                        images.Add(new Image
                        {
                            ImageId = Guid.NewGuid().ToString(),
                            ImageName = cloudinaryModel.FileName,
                            Url = cloudinaryModel.Url,
                            ProductId = productId
                        });
                    }
                    // thêm sản phẩm
                    var codeNumber = _productService.Table().Count() == 0 ?
                        1000 : _productService.Table().Max(p => p.CodeNumber) + 1;
                    var product = new Product
                    {
                        ProductId = productId,
                        ProductCode = "SP" + DateTime.Now.Year.ToString() + codeNumber,
                        CodeNumber = codeNumber,
                        ProductName = createProduct.ProductName,
                        ProductSlug = productSlug,
                        Price = createProduct.Price ?? 0,
                        PercentDiscount = createProduct.PercentDiscount,
                        Description = createProduct.Description,
                        Quantity = createProduct.Quantity ?? 0,
                        IsActive = createProduct.IsActive,
                        BrandId = createProduct.BrandId,
                    };
                    List<CategoryProducts> cateProducts = new List<CategoryProducts>();
                    foreach(var cateId in createProduct.CategoryIds)
                    {
                        cateProducts.Add(new CategoryProducts
                        {
                            ProductId = productId,
                            CategoryId = cateId,
                        });
                    }
                    List<AuthorProduct> authorProducts = new List<AuthorProduct>();
                    if(createProduct.AuthorIds != null)
                    {
                        foreach(var authorId in createProduct.AuthorIds)
                        {
                            authorProducts.Add(new AuthorProduct
                            {
                                ProductId = productId,
                                AuthorId = authorId,
                            });
                        }
                    }
                    await _context.Database.BeginTransactionAsync();
                    await _productService.AddAsync(product, images, cateProducts, authorProducts);
                    await _context.Database.CommitTransactionAsync();
                    return Ok(new {mesClient = "Bạn vừa thêm một sản phẩm", mesDev = "add product successfully"});
                }
                catch
                {
                    await _context.Database.RollbackTransactionAsync();
                    return BadRequest(new {isvalid = true, mesClient = "Xảy ra lỗi khi thêm sản phẩm", mesDev = "Error when add product"});
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

            return BadRequest(new { error = error, isvalid = false });
        }
    }
}
