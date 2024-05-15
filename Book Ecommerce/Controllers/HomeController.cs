using Book_Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using Book_Ecommerce.Domain.Helpers;
using Book_Ecommerce.Domain.ViewModels;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;

namespace Book_Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManage;
        private readonly IAuthorService _authorService;
        private readonly IBannerService _bannerService;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;

        public HomeController(ILogger<HomeController> logger, 
            AppDbContext context, 
            UserManager<AppUser> userManager,
            IAuthorService authorService,
            IBannerService bannerService,
            IProductService productService,
            IBrandService brandService)
        {
            _logger = logger;
            _context = context;
            _userManage = userManager;
            _authorService = authorService;
            _bannerService = bannerService;
            _productService = productService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var authors = await _authorService.GetDataAsync();
                var brands = await _brandService.GetDataAsync();
                var newProduct = await _productService.Table()
                                                    .Include(p => p.Images)
                                                    .OrderByDescending(p => p.CodeNumber)
                                                    .Take(10).ToListAsync();
                var products = await _productService.Table()
                                                    .Include(p => p.Images)
                                                    .Take(24).ToListAsync();
                var topSelling = await _productService.GetTopSelling();
                var homeVM = new HomeVM
                {
                    Authors = authors,
                    Brands = brands,
                    NewProducts = newProduct.Select(p => new ProductVM
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        ProductName = p.ProductName,
                        ProductSlug = p.ProductSlug,
                        Quantity = p.Quantity,
                        Price = p.Price,
                        PriceAfterDiscount = (p.PercentDiscount > 0) ? p.Price - (p.Price * ((decimal)p.PercentDiscount / 100)) : p.Price,
                        PercentDiscount = p.PercentDiscount,
                        Decription = p.Description,
                        Images = p.Images
                    }).ToList(),
                    TopSelling = topSelling,
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
                        Decription = p.Description,
                        Images = p.Images
                    }).ToList(),
                };
                return View(homeVM);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
