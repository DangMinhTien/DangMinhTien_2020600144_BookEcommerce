using Book_Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Data.Abstract
{
    public interface IUnitOfWork
    {
        IRepository<Product> ProductRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<CategoryProducts> CategoryProductRepository { get; }
        IRepository<Author> AuthorRepository { get; }
        IRepository<AuthorProduct> AuthorProductRepository { get; }
        IRepository<Brand> BrandRepository { get; }
        IRepository<AppUser> UserRepository { get; }
        IRepository<IdentityRole> RoleRepository { get; }
        IRepository<Customer> CustomerRepository { get; }
        IRepository<CartItem> CartItemRepository { get; }
        IRepository<Province> ProvinceRepository { get; }
        IRepository<District> DistrictRepository { get; }
        IRepository<Ward> WardRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<OrderDetail> OrderDetailRepository { get; }
        IRepository<Image> ImageRepository { get; }
        IRepository<Banner> BannerRepository { get; }
        IRepository<Employee> EmployeeRepository { get; }
        IRepository<Comment> CommentRepository { get; }

        Task SaveChangesAsync();
    }
}
