using Book_Ecommerce.Data;
using Book_Ecommerce.MySettings;
using Book_Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
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
                    Size = 30,
                },
                new PageSize
                {
                    Size = 50,
                }
            };
        }
        [HttpGet("/quan-ly-san-pham.html")]
        public async Task<IActionResult> Index(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _context.Products.Include(p => p.Images).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
                ViewBag.search = search;
            }
            var products = await query.ToListAsync();

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
                IsActive = p.IsActive,
                Decription = p.Decription,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        Url.Action("Index", "Products", new { page = p, pagesize = pagesize }) ?? "" :
                                        Url.Action("Index", "Products", new { page = p, search = search, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                                        Url.Action("Index", "Products", new { page = page, pagesize = psItem.Size }) ?? "" :
                                        Url.Action("Index", "Products", new { page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            ViewBag.pagingModel = pagingModel;
            ViewBag.pageSizes = lstPageSize;
            #endregion kết thúc phân trang
            var returnUrl = Url.Action("Index", "Products", new 
            { 
                area = "Admin", 
                page = page, 
                search = search, 
                pagesize = pagesize 
            });
            ViewBag.returnUrl = returnUrl;
            return View(result);
        }
        [HttpGet("/quan-ly-san-pham/tao-moi.html")]
        public async Task<IActionResult> Create(string? returnUrl)
        {
            var categories = await _context.Categories.ToListAsync();
            var authors = await _context.Authors.ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            ViewBag.categories = new MultiSelectList(categories, "CategoryId", "CategoryName");
            ViewBag.authors = new MultiSelectList(authors, "AuthorId", "AuthorName");
            ViewBag.brands = new SelectList(brands, "BrandId", "BrandName");
            ViewBag.returnUrl = returnUrl;
            return View();
        }
    }
}
