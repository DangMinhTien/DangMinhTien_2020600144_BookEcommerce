using Book_Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    public class SeedDataController : Controller
    {
        private readonly AppDbContext _context;

        public SeedDataController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                string[] categoryNames = new string[] { "Sách thiếu nhi", "Sách giáo khoa", "Sách khoa học" };
                string[] brandNames = new string[] { "Nhà xuất bản Kim đồng", "Nhà xuất bản tuổi trẻ", "Nhà xuất bản Giáo dục" };
                string[] authorNames = new string[] { "Nguyễn Văn Nhiên", "Meteo Messi", "Hoàng Công linh" };
                var maxCodeCate = 1000;
                var maxCodeBrand = 1000;
                var maxCodeAuthor = 1000;
                var maxCodeProduct = 1000;
                var countImage = 1;
                for (int i = 0; i < 3; i++)
                {
                    var category = new Category
                    {
                        CategoryId = Guid.NewGuid().ToString(),
                        CategoryCode = "TL" + DateTime.Now.Year.ToString() + maxCodeCate,
                        MaxCodeNumber = maxCodeCate++,
                        CategoryName = categoryNames[i],
                        CategorySlug = Book_Ecommerce.Helpers.Generation.GenerationSlug(categoryNames[i]),
                        Decription = "Thể loại thú vị"
                    };
                    var brand = new Brand
                    {
                        BrandId = Guid.NewGuid().ToString(),
                        BrandCode = "TH" + DateTime.Now.Year.ToString() + maxCodeBrand,
                        MaxCodeNumber = maxCodeBrand++,
                        BrandName = brandNames[i],
                        BrandSlug = Book_Ecommerce.Helpers.Generation.GenerationSlug(brandNames[i]),
                        Decription = "Thương hiệu tuyệt vời",
                        Image = Book_Ecommerce.Helpers.Generation.GenerationSlug(brandNames[i]) + ".png"
                    };
                    var author = new Author
                    {
                        AuthorId = Guid.NewGuid().ToString(),
                        AuthorCode = "TH" + DateTime.Now.Year.ToString() + maxCodeAuthor,
                        MaxCodeNumber = maxCodeAuthor++,
                        AuthorName = authorNames[i],
                        AuthorSlug = Book_Ecommerce.Helpers.Generation.GenerationSlug(authorNames[i]),
                        Information = "Tác giả lừng danh",
                    };
                    _context.Categories.Add(category);
                    _context.Brands.Add(brand);
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();
                    for (int j = 0; j < 7; j++)
                    {
                        var product = new Product
                        {
                            ProductId = Guid.NewGuid().ToString(),
                            ProductCode = "SP" + DateTime.Now.Year.ToString() + maxCodeProduct,
                            MaxCodeNumber = maxCodeProduct++,
                            ProductName = categoryNames[i] + " " + (j + 1),
                            ProductSlug = Book_Ecommerce.Helpers.Generation.GenerationSlug(categoryNames[i] + " " + (j + 1)),
                            Quantity = 10,
                            Price = 200000,
                            PercentDiscount = 0,
                            IsActive = true,
                            Decription = "Sản phẩm chất lượng cao",
                            BrandId = brand.BrandId
                        };
                        _context.Products.Add(product);
                        await _context.SaveChangesAsync();
                        for (int z = 0; z < 3; z++)
                        {
                            var image = new Image
                            {
                                ImageId = Guid.NewGuid().ToString(),
                                ImageName = $"image{countImage++}.png",
                                ProductId = product.ProductId,
                            };
                            _context.Images.Add(image);
                            await _context.SaveChangesAsync();
                        }
                        var categories = await _context.Categories.ToListAsync();
                        foreach(var cate in  categories)
                        {
                            var categoryProduct = new CategoryProducts
                            {
                                ProductId = product.ProductId,
                                CategoryId = cate.CategoryId,
                            };
                            _context.CategoryProducts.Add(categoryProduct);
                            await _context.SaveChangesAsync();
                        }
                        var authors = await _context.Authors.ToListAsync();
                        foreach (var au in authors)
                        {
                            var authorProduct = new AuthorProduct
                            {
                                ProductId = product.ProductId,
                                AuthorId = au.AuthorId,
                            };
                            _context.AuthorProducts.Add(authorProduct);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message.ToString();
            }
            TempData["success"] = "Tự động tạo dữ liệu thành công";
            return RedirectToAction("Index","Home");
        }
    }
}
