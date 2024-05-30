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
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MyRole.EMPLOYEE}, {MyRole.ADMIN}")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IBrandService _brandService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IImageService _imageService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IAuthorProductService _authorProductService;
        private readonly ICategoryProductService _categoryProductService;

        public ProductsController(AppDbContext context,
            IProductService productService,
            ICategoryService categoryService,
            IAuthorService authorService,
            IBrandService brandService,
            ICloudinaryService cloudinaryService,
            IImageService imageService,
            ILogger<ProductsController> logger,
            IAuthorProductService authorProductService,
            ICategoryProductService categoryProductService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _authorService = authorService;
            _brandService = brandService;
            _cloudinaryService = cloudinaryService;
            _imageService = imageService;
            _logger = logger;
            _authorProductService = authorProductService;
            _categoryProductService = categoryProductService;
        }
        [HttpGet("/quan-ly-san-pham")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.search = search;
            }
            (var productVMs, var pagingModel, var lstPageSize) = await _productService.GetToViewManageAsync(search, page, pagesize);
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            ViewBag.pagesize = pagesize;
            var returnUrl = Url.Action("Index", "Products", new 
            { 
                area = "Admin", 
                page = pagingModel.currentpage, 
                search = search, 
                pagesize = pagesize 
            });
            ViewBag.returnUrl = returnUrl;
            return View(productVMs);
        }
        [HttpGet("/quan-ly-san-pham/them-moi")]
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
        [HttpPost("/quan-ly-san-pham/them-moi")]
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
                catch(Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return BadRequest(new {isValid = true, mesClient = "Xảy ra lỗi khi thêm sản phẩm", mesDev = ex.Message});
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
        [HttpGet("/quan-ly-san-pham/capnhat/{productCode}")]
        public async Task<IActionResult> Edit(string productCode, string? returnUrl)
        {
            var productVMs = await _productService.GetDetailToViewManageAsync(productCode);
            if(productVMs == null)
            {
                return NotFound($"Sản phẩm có mã {productCode} không tồn tại");
            }
            var authors = await _authorService.GetDataAsync();
            var authorIdsSelected = productVMs.Authors.Select(a => a.AuthorId).ToList();
            var categories = await _categoryService.GetDataAsync();
            var categoryIdsSelected = productVMs.Categories.Select(c => c.CategoryId).ToList();
            var brands = await _brandService.GetDataAsync();
            ViewBag.authors = new MultiSelectList(authors, "AuthorId", "AuthorName", authorIdsSelected);
            ViewBag.categories = new MultiSelectList(categories, "CategoryId", "CategoryName", categoryIdsSelected);
            ViewBag.brands = new SelectList(brands, "BrandId", "BrandName", productVMs.Brand.BrandId);
            ViewBag.returnUrl = returnUrl;
            return View(productVMs);
        }
        [HttpPost("/quan-ly-san-pham/capnhat/{productId}")]
        public async Task<IActionResult> Update(string productId, EditProduct editProduct)
        {
            if (_productService.Table().Any(p => p.ProductName == editProduct.ProductName && p.ProductId != productId))
            {
                ModelState.AddModelError(string.Empty, "Tên sản phẩm vừa nhập bị trùng với một sản phẩm khác");
            }
            if(ModelState.IsValid)
            {
                try
                {
                    var product = await _productService.Table()
                                                        .Include(p => p.AuthorProducts)
                                                        .Include(p => p.CategoryProducts)
                                                        .FirstOrDefaultAsync(p => p.ProductId == productId);
                    if (product == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Sản phẩm không tồn tại", mesDev = "Product is not found" });
                    }
                    // sửa lại authorProduct
                    var authorProducts = product.AuthorProducts;
                    var authorIdsSelected = editProduct.AuthorIds ?? new List<string>();
                    var authorProductsDelete = authorProducts.Where(a => !authorIdsSelected.Contains(a.AuthorId)).ToList();
                    var authorIdsAdd = authorIdsSelected.Where(a => !authorProducts.Any(ap => ap.AuthorId == a)).ToList();
                    var authorProductAdd = new List<AuthorProduct>();
                    foreach(var authorId in authorIdsAdd)
                    {
                        authorProductAdd.Add(new AuthorProduct
                        {
                            AuthorId = authorId,
                            ProductId = product.ProductId
                        });
                    }
                    // sửa lại categoryProduct
                    var categoryProduct = product.CategoryProducts;
                    var categoryIdsSelected = editProduct.CategoryIds;
                    var categoryProductDelete = categoryProduct.Where(cp => !categoryIdsSelected.Contains(cp.CategoryId)).ToList();
                    var categoryIdsAdd = categoryIdsSelected.Where(c => !categoryProduct.Any(cp => cp.CategoryId == c)).ToList();
                    var categoryProductAdd = new List<CategoryProducts>();
                    foreach(var categoryId in categoryIdsAdd)
                    {
                        categoryProductAdd.Add(new CategoryProducts
                        {
                            CategoryId = categoryId,
                            ProductId = product.ProductId
                        });
                    }
                    // sửa lại thông tin sản phẩm
                    var productSlug = string.Empty;
                    do
                    {
                        productSlug = Generation.GenerationSlug(editProduct.ProductName);
                    } while (_productService.Table().Any(p => p.ProductSlug == productSlug && p.ProductId != product.ProductId));
                    product.ProductSlug = productSlug;
                    product.ProductName = editProduct.ProductName;
                    product.Price = editProduct.Price ?? 0;
                    product.PercentDiscount = editProduct.PercentDiscount;
                    product.Description = editProduct.Description;
                    product.Quantity = editProduct.Quantity ?? 0;
                    product.IsActive = editProduct.IsActive;
                    product.BrandId = editProduct.BrandId;
                    await _context.Database.BeginTransactionAsync();
                    if(authorProductsDelete.Count > 0)
                    {
                        await _authorProductService.RemoveRangeAsync(authorProductsDelete);
                    }
                    if(authorProductAdd.Count > 0)
                    {
                        await _authorProductService.AddRangeAsync(authorProductAdd);
                    }
                    if(categoryProductDelete.Count > 0)
                    {
                        await _categoryProductService.RemoveRangeAsync(categoryProductDelete);
                    }
                    if(categoryProductAdd.Count > 0)
                    {
                        await _categoryProductService.AddRangeAsync(categoryProductAdd);
                    }
                    await _productService.UpdateAsync(product);
                    await _context.Database.CommitTransactionAsync();
                    return Ok(new {mesClient = $"Sửa sản phẩm {product.ProductCode} thành công ", mesDev = $"Update product {product.ProductCode} complete "});
                }
                catch(Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return BadRequest(new {isValid = true, mesClient = "Không thể sửa được sản phẩm", mesDev = ex.Message});
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
        [HttpGet]
        public async Task<IActionResult> GetImageByProduct(string productId)
        {
            try
            {
                var images = await _imageService.GetImageByProductAsync(productId);
                var imageVMs = images.Select(i => new
                {
                    url = i.Url,
                    imageName = i.ImageName,
                    productId = i.ProductId,
                    imageId = i.ImageId
                }).ToList();
                return Json(new {images = imageVMs});
            }
            catch(Exception ex)
            {
                return BadRequest(new {mesClient = "Không lấy được ảnh của sản phẩm", mesDev = ex.Message});
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteImage(string imageId)
        {
            try
            {
                var image = await _imageService.GetSingleByConditionAsync(i => i.ImageId == imageId);
                if(image == null)
                {
                    return BadRequest(new { mesClient = "Không tìm thấy ảnh cần xóa", mesDev = "Can not find image to delete" });
                }
                await _imageService.RemoveAsync(image);
                try
                {
                    await _cloudinaryService.DeleteAsync(image.ImageName);
                }
                catch(Exception ex)
                {
                    _logger.LogInformation($"Có lỗi khi xóa ảnh trong cloudinary ({ex.Message})");
                }
                return Ok(new { mesClient = "Xóa ảnh thành công", mesDev = "delete image complete" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { mesClient = "Không xóa được ảnh của sản phẩm", mesDev = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddImage(IFormFile file, string productId)
        {
            try
            {
                var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == productId);
                if(product == null)
                {
                    return BadRequest(new {mesClient = "Không thể thêm ảnh do không tìm thấy sản phẩm", mesDev = "can not add image because product is not found"});
                }
                var cloudinaryModel = await _cloudinaryService.UploadAsync(file);
                var image = new Image
                {
                    ImageId = Guid.NewGuid().ToString(),
                    Url = cloudinaryModel.Url,
                    ImageName = cloudinaryModel.FileName,
                    ProductId = product.ProductId
                };
                await _imageService.AddAsync(image);
                return Ok(new {mesClient = "Thêm ảnh thành công", mesDev = "add image is successfully"});
            }
            catch(Exception ex)
            {
                return BadRequest(new { mesClient = "Không thể thêm ảnh do lỗi hệ thống", mesDev = ex.Message });
            }
        }
        [HttpPost("/quan-ly-san-pham/xoa")]
        public async Task<IActionResult> Delete(string productId)
        {
            try
            {
                var product = await _productService.Table()
                                                    .Include(p => p.Images)
                                                    .FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    return BadRequest(new { mesClient = "Xóa thất bại do sản phẩm không tồn tại", mesDev = "Product is not found" });
                }
                var ImageNames = product.Images.Select(p => p.ImageName).ToList();
                await _productService.RemoveAsync(product);
                try
                {
                    await _cloudinaryService.DeleteRangeAsync(ImageNames);
                }
                catch
                {
                    _logger.LogInformation("cloudinary bị lỗi");
                }
                TempData["success"] = "Xóa sản phẩm vừa chọn thành công";
                return Ok(new { mesClient = "Xóa sản phẩm thành công", mesDev = "Delete product complete" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { mesClient = "Xóa thất bại do lỗi hệ thống", mesDev = ex.Message });
            }
        }
    }
}
