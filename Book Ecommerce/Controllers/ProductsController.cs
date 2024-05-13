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
using Microsoft.AspNetCore.Identity;


namespace Book_Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IAuthorService _authorService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly ICommentService _commentService;

        public ProductsController(AppDbContext context, 
            IProductService productService,
            ICategoryService categoryService,
            IBrandService brandService,
            IAuthorService authorService,
            UserManager<AppUser> userManager,
            ICustomerService customerService,
            ICommentService commentService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _authorService = authorService;
            _userManager = userManager;
            _customerService = customerService;
            _commentService = commentService;
        }
        [HttpGet("/sanpham")]
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
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/sanpham/theloai/{categorySlug}")]
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
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/sanpham/thuonghieu/{brandSlug}")]
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
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/sanpham/tacgia/{authorSlug}")]
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
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/sanpham/{productSlug}")]
        public async Task<IActionResult> Detail(string productSlug)
        {
            try
            {
                var product = await _productService.GetDetailToViewAsync(productSlug);
                if (product == null)
                {
                    return NotFound("Không tìm thấy sản phẩm");
                }
                return View(product);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/sanpham/laydanhgia")]
        public async Task<IActionResult> GetComment(string productId)
        {
            try
            {
                var product = await _productService.GetSingleByConditionAsync(c => c.ProductId == productId);
                if (product == null)
                {
                    return BadRequest(new { mesClient = "Không lấy được đánh giá sản phẩm do không tìm thấy sản phẩm", mesDev = "Product is not found" });
                }
                var commnents = await _commentService.Table().Include(c => c.Customer)
                                                        .Where(c => c.ProductId == product.ProductId)
                                                        .OrderByDescending(c => c.DateCreated).ToListAsync();
                var result = commnents.Select(c => new
                {
                    commentId = c.CommentId,
                    vote = c.Vote,
                    message = c.Message,
                    customerName = c.Customer.FullName,
                    dateCreated = c.DateCreated.ToString("dd/MM/yyyy - HH:mm:ss"),
                });
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new {mesClient = "Không lấy được đánh giá do lỗi hệ thống", mesDev =  ex.Message});
            }
        }
        [Authorize(Roles = MyRole.CUSTOMER)]
        [HttpPost("/sanpham/guidanhgia")]
        public async Task<IActionResult> SendComment(string productId, InputComment inputComment)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không gửi được đánh giá do không tìm thấy tài khoản đăng nhập", mesDev = "don't find account login" });
                    }
                    var customer = await _customerService.GetSingleByConditionAsync(c => c.CustomerId == user.CustomerId);
                    if (customer == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không gửi được đánh giá do không tìm thấy khách hàng đăng nhập", mesDev = "don't find customer login" });
                    }
                    var product = await _productService.GetSingleByConditionAsync(p => p.ProductId == productId);
                    if (product == null)
                    {
                        return BadRequest(new { isValid = true, mesClient = "Không gửi được đánh giá do không tìm thấy sản phẩm", mesDev = "don't find product" });
                    }
                    var comment = new Comment
                    {
                        CommentId = Guid.NewGuid().ToString(),
                        Vote = inputComment.Vote ?? 5,
                        Message = inputComment.Message,
                        DateCreated = DateTime.Now,
                        CustomerId = customer.CustomerId,
                        ProductId = product.ProductId,
                    };
                    await _commentService.AddAsync(comment);
                    return Ok(new {mesClient = "Gửi đánh giá thành công", mesDev = "send comment successfully"});
                }
                catch(Exception ex)
                {
                    return BadRequest(new { isValid = true, mesClient = "Không gửi được đánh giá do lỗi hệ thống", mesDev = ex.Message });
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
            return BadRequest(new {isValid = false, error = error, mesClient = "Lỗi nhập dữ liệu", mesDev = "error inpur data"});
        }
    }
}
