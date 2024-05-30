using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace Book_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = MyRole.ADMIN)]
    public class StatisticalController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public StatisticalController(IOrderService orderService, 
            ICategoryService categoryService,
            IBrandService brandService,
            IProductService productService,
            IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _categoryService = categoryService;
            _brandService = brandService;
            _productService = productService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("/thong-ke")]
        public IActionResult Index()
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo("vi-VN");
                var sumQuantityProduct = _productService.Table().Sum(p => p.Quantity);
                var sumProductBuy = _unitOfWork.OrderDetailRepository.Table().Sum(o => o.Quantity);
                var revenue = _unitOfWork.OrderDetailRepository.Table().Sum(o => o.Quantity * o.Price) 
                    + _unitOfWork.OrderRepository.Table().Sum(o => o.TransportFee);
                ViewBag.sumQuantityProduct = sumQuantityProduct;
                ViewBag.sumProductBuy = sumProductBuy;
                ViewBag.revenue = string.Format(cultureInfo, "{0:C0}", revenue);
                return View();
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/thong-ke/laydoanhthu")]
        public async Task<IActionResult> GetRevenue(int year)
        {
            try
            {
                List<int> lstMonth = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
                var result = new List<dynamic>();
                var orders = await _orderService.Table()
                    .Include(o => o.OrderDetails).Where(o => o.DateCreated.Year == year).ToListAsync();
                var resultMonthContain = orders.GroupBy(o => o.DateCreated.Month)
                                .Select(group => new
                                {
                                    month = group.Key,
                                    totalQuantity = group.Sum(o => o.OrderDetails.Sum(od => od.Quantity)),
                                    revenue = group.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price) + o.TransportFee)
                                }).ToList();
                foreach(var month in lstMonth)
                {
                    var revenueInMonth = resultMonthContain.FirstOrDefault(r => r.month == month);
                    if (revenueInMonth != null)
                    {
                        result.Add(new
                        {
                            month = revenueInMonth.month,
                            totalQuantity = revenueInMonth.totalQuantity,
                            revenue = revenueInMonth.revenue
                        });
                    }
                    else
                    {
                        result.Add(new
                        {
                            month = month,
                            totalQuantity = 0,
                            revenue = 0
                        });
                    }
                }
                //foreach (var month in lstMonth)
                //{
                //    var orders = await _orderService.Table().Include(o => o.OrderDetails)
                //        .Where(o => o.DateCreated.Year == year && o.DateCreated.Month == month)
                //        .ToListAsync();
                //    result.Add(new
                //    {
                //        month = month,
                //        totalQuantity = orders.Sum(o => o.OrderDetails.Sum(od => od.Quantity)),
                //        revenue = orders.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price) + o.TransportFee)
                //    });
                //}
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new {mesClient = "Không lấy được doanh thu", mesDev = ex.Message});
            }
        }
        [HttpGet("/thong-ke/topbanchay")]
        public async Task<IActionResult> getTopSelling(int year)
        {
            try
            {
                var products = await _orderService.TableOrderDetail().Include(od => od.Product)
                                            .GroupBy(od => od.ProductId)
                                            .OrderByDescending(g => g.Sum(od => od.Quantity))
                                            .Select(g => new
                                            {
                                                productId = g.First().Product.ProductId,
                                                productName = g.First().Product.ProductName,
                                                totalQuantity = g.Sum(od => od.Quantity)
                                            }).Take(10).ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được doanh thu", mesDev = ex.Message });
            }
        }
        [HttpGet("/thong-ke/theloai")]
        public async Task<IActionResult> Category()
        {
            try
            {
                var result = await _categoryService.Table().Include(c => c.CategoryProducts)
                                                    .Select(c => new
                                                    {
                                                        categoryName = c.CategoryName,
                                                        totalProduct = c.CategoryProducts.Count()
                                                    }).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được doanh thu", mesDev = ex.Message });
            }
        }
        [HttpGet("/thong-ke/thuonghieu")]
        public async Task<IActionResult> Brand()
        {
            try
            {
                var result = await _brandService.Table().Include(b => b.Products)
                                                    .Select(b => new
                                                    {
                                                        brandName = b.BrandName,
                                                        totalProduct = b.Products.Count()
                                                    }).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesClient = "Không lấy được doanh thu", mesDev = ex.Message });
            }
        }
    }
}
