using Book_Ecommerce.Data;
using Book_Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    public class SeedDataController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDataController(UserManager<AppUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                        CodeNumber = maxCodeCate++,
                        CategoryName = categoryNames[i],
                        CategorySlug = Book_Ecommerce.Helpers.Generation.GenerationSlug(categoryNames[i]),
                        Decription = "Thể loại thú vị"
                    };
                    var brand = new Brand
                    {
                        BrandId = Guid.NewGuid().ToString(),
                        BrandCode = "TH" + DateTime.Now.Year.ToString() + maxCodeBrand,
                        CodeNumber = maxCodeBrand++,
                        BrandName = brandNames[i],
                        BrandSlug = Book_Ecommerce.Helpers.Generation.GenerationSlug(brandNames[i]),
                        Decription = "Thương hiệu tuyệt vời",
                        Image = Book_Ecommerce.Helpers.Generation.GenerationSlug(brandNames[i]) + ".png"
                    };
                    var author = new Author
                    {
                        AuthorId = Guid.NewGuid().ToString(),
                        AuthorCode = "TH" + DateTime.Now.Year.ToString() + maxCodeAuthor,
                        CodeNumber = maxCodeAuthor++,
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
                            CodeNumber = maxCodeProduct++,
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
                TempData["success"] = "Tự động tạo dữ liệu thành công";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message.ToString();
            }
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> RenderUserRole()
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var roleNames = new string[] { MyRole.CUSTOMER, MyRole.EMPLOYEE, MyRole.ADMIN };
                var roles = await _context.Roles.Select(r => r.Name.ToUpper()).ToListAsync();
                var roleAdd = roleNames.Where(r => !roles.Contains(r.ToUpper())) ?? new string[] { };
                foreach (var roleName in roleAdd)
                {
                    var resultRole = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                    if(!resultRole.Succeeded)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        TempData["error"] = "Tạo dữ liệu thất bại do không tạo được quyền";
                        return RedirectToAction("Index","Home");
                    }
                }
                // tao customer 
                var customerCode = _context.Customers.Count() == 0 ? 1000
                    : _context.Customers.Max(c => c.CodeNumber) + 1;
                var customer = new Customer
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    FullName = "Đặng Tiến",
                    CodeNumber = customerCode,
                    CustomerCode = "KH" + customerCode,
                    Gender = true,
                    DateOfBirth = DateTime.Now,
                };
                _context.Customers.Add(customer);
                var userCustomer = new AppUser
                {
                    UserName = "customer@gmail.com",
                    Email = "customer@gmail.com",
                    EmailConfirmed = true,
                    CustomerId = customer.CustomerId
                };
                var resultCustomerAccount = await _userManager.CreateAsync(userCustomer, "@Tien2801");
                if (!resultCustomerAccount.Succeeded)
                {
                    await _context.Database.RollbackTransactionAsync();
                    TempData["error"] = "Tạo dữ liệu thất bại do không tạo được tài khoản khách hàng";
                    return RedirectToAction("Index", "Home");
                }
                // add role customer
                var resultAddRoleCustomer = await _userManager
                    .AddToRoleAsync(userCustomer, MyRole.CUSTOMER);
                if (!resultAddRoleCustomer.Succeeded)
                {
                    await _context.Database.RollbackTransactionAsync();
                    TempData["error"] = "Tạo dữ liệu thất bại do không thêm được quyền tài khoản khách hàng";
                    return RedirectToAction("Index", "Home");
                }
                // tạo employee
                var employeeCode = _context.Employees.Count() == 0 ? 1000 :
                    _context.Employees.Max(e => e.CodeNumber) + 1;
                var employee = new Employee
                {
                    EmployeeId = Guid.NewGuid().ToString(),
                    FullName = "Đặng Minh Tiến",
                    DateOfBirth = DateTime.Now,
                    Gender = true,
                    CodeNumber = employeeCode,
                    EmployeeCode = "NV" + employeeCode,
                    Address = "AD-HP-VN"
                };
                _context.Employees.Add(employee);
                var userEmployee = new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    EmployeeId = employee.EmployeeId
                };
                var resultUserEmployee = await _userManager.CreateAsync(userEmployee, "@Tien2801");
                if (!resultUserEmployee.Succeeded)
                {
                    await _context.Database.RollbackTransactionAsync();
                    TempData["error"] = "Tạo dữ liệu thất bại do không tạo được tài khoản nhân viên";
                    return RedirectToAction("Index", "Home");
                }
                // add role employee
                var addRoleEmployee = new string[] { MyRole.EMPLOYEE, MyRole.ADMIN };
                foreach(var role in addRoleEmployee)
                {
                    var resultAddRoleEmployee = await _userManager.AddToRoleAsync(userEmployee, role);
                    if(!resultAddRoleEmployee.Succeeded)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        TempData["error"] = "Tạo dữ liệu thất bại do không thêm được quyền vào tài khoản nhân viên";
                        return RedirectToAction("Index", "Home");
                    }
                }
                await _context.Database.CommitTransactionAsync();
                TempData["success"] = "Tạo dữ liệu thành công";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
