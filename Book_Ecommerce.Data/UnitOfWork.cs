using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IRepository<Product> ProductRepository => new Repository<Product>(_dbContext);
        public IRepository<Category> CategoryRepository => new Repository<Category>(_dbContext);
        public IRepository<CategoryProducts> CategoryProductRepository 
            => new Repository<CategoryProducts>(_dbContext);
        public IRepository<Author> AuthorRepository => new Repository<Author>(_dbContext);
        public IRepository<AuthorProduct> AuthorProductRepository 
            => new Repository<AuthorProduct>(_dbContext);
        public IRepository<Brand> BrandRepository => new Repository<Brand>(_dbContext);
        public IRepository<AppUser> UserRepository => new Repository<AppUser>(_dbContext);
        public IRepository<IdentityRole> RoleRepository => new Repository<IdentityRole>(_dbContext);
        public IRepository<Customer> CustomerRepository => new Repository<Customer>(_dbContext);
        public IRepository<CartItem> CartItemRepository => new Repository<CartItem>(_dbContext);
        public IRepository<Province> ProvinceRepository => new Repository<Province>(_dbContext);
        public IRepository<District> DistrictRepository => new Repository<District>(_dbContext);
        public IRepository<Ward> WardRepository => new Repository<Ward>(_dbContext);
        public IRepository<Order> OrderRepository => new Repository<Order>(_dbContext);
        public IRepository<OrderDetail> OrderDetailRepository => new Repository<OrderDetail>(_dbContext);
        public IRepository<Image> ImageRepository => new Repository<Image>(_dbContext);
        public IRepository<Banner> BannerRepository => new Repository<Banner>(_dbContext);
        public IRepository<Employee> EmployeeRepository => new Repository<Employee>(_dbContext);
        public IRepository<Comment> CommentRepository => new Repository<Comment>(_dbContext);
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
