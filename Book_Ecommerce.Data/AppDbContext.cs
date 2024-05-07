using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Domain.Entities;

namespace Book_Ecommerce.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Banner> Banners { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<FavouriteProduct> Favourites { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<CategoryProducts> CategoryProducts { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<AuthorProduct> AuthorProducts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Province> Provinces { get; set; } = null!;
        public DbSet<District> Districts { get; set; } = null!;
        public DbSet<Ward> Wards { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName() ?? "";
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(au => au.PhoneNumber).IsUnique();
                entity.HasIndex(au => au.Email).IsUnique();
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.CategoryCode).IsUnique();
                entity.HasIndex(c => c.CategorySlug).IsUnique();
                entity.HasIndex(c => c.CategoryName).IsUnique();
                entity.HasIndex(c => c.CodeNumber).IsUnique();
            });
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(b => b.BrandCode).IsUnique();
                entity.HasIndex(b => b.BrandSlug).IsUnique();
                entity.HasIndex(b => b.BrandName).IsUnique();
                entity.HasIndex(b => b.CodeNumber).IsUnique();
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.ProductCode).IsUnique();
                entity.HasIndex(p => p.ProductSlug).IsUnique();
                entity.HasIndex(p => p.ProductName).IsUnique();
                entity.HasIndex(p => p.CodeNumber).IsUnique();
                entity.HasOne(p => p.Brand)
                        .WithMany(b => b.Products)
                        .HasForeignKey(p => p.BrandId)
                        .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasOne(i => i.Product)
                        .WithMany(p => p.Images)
                        .HasForeignKey(i => i.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(c => c.CustomerCode).IsUnique();
                entity.HasIndex(c => c.CodeNumber).IsUnique();
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                entity.HasIndex(e => e.CodeNumber).IsUnique();
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(o => o.OrderCode).IsUnique();
                entity.HasIndex(o => o.CodeNumber).IsUnique();
                entity.HasOne(o => o.Customer)
                        .WithMany(c => c.Orders)
                        .HasForeignKey(o => o.CustomerId)
                        .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => new { od.OrderId, od.ProductId});
                entity.HasOne(od => od.Order)
                        .WithMany(o => o.OrderDetails)
                        .HasForeignKey(od => od.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Product)
                        .WithMany(p => p.OrderDetails)
                        .HasForeignKey(od => od.ProductId)
                        .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FavouriteProduct>(entity =>
            {
                entity.HasKey(f => new { f.CustomerId, f.ProductId });
                entity.HasOne(f => f.Customer)
                        .WithMany(c => c.FavouriteProducts)
                        .HasForeignKey(f => f.CustomerId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(f => f.Product)
                        .WithMany(p => p.FavouriteProducts)
                        .HasForeignKey(f => f.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Customer)
                        .WithMany(cu => cu.Comments)
                        .HasForeignKey(c => c.CustomerId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Product)
                        .WithMany(p => p.Comments)
                        .HasForeignKey(c => c.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasOne(u => u.Customer)
                        .WithOne(c => c.User)
                        .HasForeignKey<AppUser>(u => u.CustomerId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(u => u.Employee)
                        .WithOne(e => e.User)
                        .HasForeignKey<AppUser>(u => u.EmployeeId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CategoryProducts>(entity =>
            {
                entity.HasKey(c => new { c.ProductId, c.CategoryId });
                entity.HasOne(c => c.Category)
                        .WithMany(c => c.CategoryProducts)
                        .HasForeignKey(c => c.CategoryId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Product)
                        .WithMany(p => p.CategoryProducts)
                        .HasForeignKey(c => c.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasIndex(a => a.AuthorCode).IsUnique();
                entity.HasIndex(a => a.AuthorSlug).IsUnique();
                entity.HasIndex(a => a.AuthorName).IsUnique();
                entity.HasIndex(a => a.CodeNumber).IsUnique();
            });
            modelBuilder.Entity<AuthorProduct>(entity =>
            {
                entity.HasKey(ap => new {ap.ProductId, ap.AuthorId});
                entity.HasOne(ap => ap.Author)
                        .WithMany(a => a.AuthorProducts)
                        .HasForeignKey(ap => ap.AuthorId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ap => ap.Product)
                        .WithMany(p => p.AuthorProducts)
                        .HasForeignKey(ap => ap.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(c => new { c.CustomerId, c.ProductId });
                entity.HasOne(c => c.Customer)
                        .WithMany(cu => cu.CartItems)
                        .HasForeignKey(c => c.CustomerId)
                        .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Product)
                        .WithMany(p => p.CartItems)
                        .HasForeignKey(c => c.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<District>(entity =>
            {
                entity.HasOne(d => d.Province)
                        .WithMany(p => p.Districts)
                        .HasForeignKey(d => d.ProvinceCode)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Ward>(entity =>
            {
                entity.HasOne(w => w.District)
                        .WithMany(d => d.Wards)
                        .HasForeignKey(w => w.DistrictCode)
                        .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
