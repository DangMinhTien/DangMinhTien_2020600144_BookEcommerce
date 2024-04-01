using Book_Ecommerce.Data;
using Book_Ecommerce.Models;
using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Book_Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly int PageSize = MyAppSetting.PAGE_SIZE;
        private readonly List<PageSize> lstPageSize;

        public ProductsController(AppDbContext context)
        {
            _context = context;
            lstPageSize = new List<PageSize>
            {
                new PageSize
                {
                    Size = MyAppSetting.PAGE_SIZE,
                },
                new PageSize
                {
                    Size = 20,
                },
                new PageSize
                {
                    Size = 50,
                }
            };
        }
        [Route("sanpham.html")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _context.Products.Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .ThenInclude(c => c.Category)
                                            .Include(p => p.Brand)
                                            .Where(p => p.IsActive)
                                            .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
                ViewBag.search = search;
            }
            var products = await query.ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if(page < 1)
                page = 1;
            if (page > totalPage)
                page = totalPage;
            var result = products.Select(p => new ProductVM
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                Quantity = p.Quantity,
                Price = p.Price,
                PriceAfterDiscount = (p.PercentDiscount > 0) ? p.Price - (p.Price * ((decimal)p.PercentDiscount / 100)) : p.Price,
                PercentDiscount = p.PercentDiscount,
                Decription = p.Decription,
                Brand = p.Brand,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ? 
                                        Url.Action("Index","Products", new {page = p, pagesize = pagesize}) ?? "":
                                        Url.Action("Index", "Products", new { page = p , search = search, pagesize = pagesize }) ?? ""
            };
            foreach(var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                                        Url.Action("Index", "Products", new { page = page, pagesize = psItem.Size }) ?? "" :
                                        Url.Action("Index", "Products", new { page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSize = lstPageSize;
            #endregion kết thúc phân trang
            return View(result);
        }
        [Route("/sanpham/theloai/{categorySlug}")]
        public async Task<IActionResult> GetByCategory(string categorySlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategorySlug == categorySlug);
            if (category == null)
            {
                return NotFound("không tìm thấy thể loại cần xem");
            }
            var products = await _context.Products.Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .ThenInclude(c => c.Category)
                                            .Include(p => p.Brand)
                                            .Where(p => p.IsActive && p.CategoryProducts.Any(c => c.CategoryId == category.CategoryId))
                                            .ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page < 1)
                page = 1;
            if (page > totalPage)
                page = totalPage;
            var result = products.Select(p => new ProductVM
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                Quantity = p.Quantity,
                Price = p.Price,
                PriceAfterDiscount = (p.PercentDiscount > 0) ? p.Price - (p.Price * ((decimal)p.PercentDiscount / 100)) : p.Price,
                PercentDiscount = p.PercentDiscount,
                Decription = p.Decription,
                Categories = p.CategoryProducts.Select(c => c.Category).ToList(),
                Brand = p.Brand,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => Url.Action("GetByCategory", "Products", new { page = p, categorySlug = categorySlug, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = Url.Action("GetByCategory", "Products", new { page = page, categorySlug = categorySlug, pagesize = psItem.Size }) ?? "";
            }
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSize = lstPageSize;
            #endregion kết thúc phân trang
            ViewBag.categoryId = category.CategoryId;
            return View("Index", result);
        }
        [Route("/sanpham/thuonghieu/{brandSlug}")]
        public async Task<IActionResult> GetByBrand(string brandSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandSlug == brandSlug);
            if (brand == null)
            {
                return NotFound("không tìm thấy thương hiệu cần xem");
            }
            var products = await _context.Products.Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .ThenInclude(c => c.Category)
                                            .Include(p => p.Brand)
                                            .Where(p => p.IsActive && p.BrandId == brand.BrandId)
                                            .ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page < 1)
                page = 1;
            if (page > totalPage)
                page = totalPage;
            var result = products.Select(p => new ProductVM
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                Quantity = p.Quantity,
                Price = p.Price,
                PriceAfterDiscount = (p.PercentDiscount > 0) ? p.Price - (p.Price * ((decimal)p.PercentDiscount / 100)) : p.Price,
                PercentDiscount = p.PercentDiscount,
                Decription = p.Decription,
                Categories = p.CategoryProducts.Select(c => c.Category).ToList(),
                Brand = p.Brand,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => Url.Action("GetbyBrand", "Products", new { page = p, brandSlug = brandSlug, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = Url.Action("GetbyBrand", "Products", new { page = page, brandSlug = brandSlug, pagesize = psItem.Size }) ?? "";
            }
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSize = lstPageSize;
            #endregion kết thúc phân trang
            ViewBag.brandId = brand.BrandId;
            return View("Index", result);
        }
        [Route("/sanpham/tacgia/{authorSlug}")]
        public async Task<IActionResult> GetByAuthor(string authorSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(c => c.AuthorSlug == authorSlug);
            if (author == null)
            {
                return NotFound("không tìm thấy tác giả cần xem");
            }
            var products = await _context.Products.Include(p => p.Images)
                                            .Include(p => p.AuthorProducts)
                                            .ThenInclude(ap => ap.Author)
                                            .Where(p => p.IsActive && p.AuthorProducts.Any(ap => ap.AuthorId == author.AuthorId))
                                            .ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page < 1)
                page = 1;
            if (page > totalPage)
                page = totalPage;
            var result = products.Select(p => new ProductVM
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                Quantity = p.Quantity,
                Price = p.Price,
                PriceAfterDiscount = (p.PercentDiscount > 0) ? p.Price - (p.Price * ((decimal)p.PercentDiscount / 100)) : p.Price,
                PercentDiscount = p.PercentDiscount,
                Decription = p.Decription,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => Url.Action("GetByAuthor", "Products", new { page = p, authorSlug = authorSlug, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = Url.Action("GetByAuthor", "Products", new { page = page, authorSlug = authorSlug, pagesize = psItem.Size }) ?? "";
            }
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSize = lstPageSize;
            #endregion kết thúc phân trang
            ViewBag.authorId = author.AuthorId;
            return View("Index", result);
        }
        [Route("/sanpham/{productSlug}.html")]
        public async Task<IActionResult> Detail(string productSlug)
        {
            var product = _context.Products.Include(p => p.Images)
                                           .Include(p => p.Brand)
                                           .Include(p => p.CategoryProducts)
                                           .ThenInclude(p => p.Category)
                                           .Include(p => p.AuthorProducts)
                                           .ThenInclude(p => p.Author)
                                           .Include(p => p.Comments)
                                           .ThenInclude(c => c.Customer)
                                           .FirstOrDefault(p => p.ProductSlug == productSlug);
            if (product == null)
            {
                return NotFound();
            }
            var categoryIds = product.CategoryProducts.Select(cp => cp.CategoryId).ToList();
            var products = await _context.Products.Include(p => p.Images)
                                                    .Include(p => p.CategoryProducts)
                                                    .Where(p => p.CategoryProducts.Any(cp => categoryIds.Contains(cp.CategoryId)) 
                                                        && p.ProductId != product.ProductId)
                                                    .ToListAsync();
            var productVM = new ProductVM
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                ProductSlug = product.ProductSlug,
                Images = product.Images,
                PriceAfterDiscount = (product.PercentDiscount > 0) ? product.Price - (product.Price * ((decimal)product.PercentDiscount / 100)) : product.Price,
                PercentDiscount = product.PercentDiscount,
                Price = product.Price,
                Decription = product.Decription,
                Quantity = product.Quantity,
                Categories = product.CategoryProducts.Select(p => p.Category).ToList(),
                Authors = product.AuthorProducts.Select(p => p.Author).ToList(),
                Brand = product.Brand,
                Comments = product.Comments,
                Products = products.Select(p => new ProductVM
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    ProductSlug = p.ProductSlug,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    PriceAfterDiscount = (p.PercentDiscount > 0) ? p.Price - (p.Price * ((decimal)p.PercentDiscount / 100)) : p.Price,
                    PercentDiscount = p.PercentDiscount,
                    Decription = p.Decription,
                    Images = p.Images
                }).ToList()
            };
            return View(productVM);
        }
    }
}
