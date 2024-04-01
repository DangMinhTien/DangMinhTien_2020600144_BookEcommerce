﻿// <auto-generated />
using System;
using Book_Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Book_Ecommerce.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240322165040_Add-First")]
    partial class AddFirst
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Book_Ecommerce.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId")
                        .IsUnique();

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Banner", b =>
                {
                    b.Property<string>("BannerId")
                        .HasColumnType("char(36)");

                    b.Property<string>("BannerName")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.HasKey("BannerId");

                    b.ToTable("Banners");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Brand", b =>
                {
                    b.Property<string>("BrandId")
                        .HasColumnType("char(36)");

                    b.Property<string>("BrandCode")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("BrandName")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("BrandSlug")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Decription")
                        .HasColumnType("nvarchar(500)");

                    b.Property<long>("MaxCodeNumber")
                        .HasColumnType("bigint");

                    b.HasKey("BrandId");

                    b.HasIndex("BrandCode")
                        .IsUnique();

                    b.HasIndex("BrandSlug")
                        .IsUnique();

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Category", b =>
                {
                    b.Property<string>("CategoryId")
                        .HasColumnType("char(36)");

                    b.Property<string>("CategoryCode")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("CategorySlug")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Decription")
                        .HasColumnType("nvarchar(500)");

                    b.Property<long>("MaxCodeNumber")
                        .HasColumnType("bigint");

                    b.HasKey("CategoryId");

                    b.HasIndex("CategoryCode")
                        .IsUnique();

                    b.HasIndex("CategorySlug")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Comment", b =>
                {
                    b.Property<string>("CommentId")
                        .HasColumnType("char(36)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<int>("Vote")
                        .HasColumnType("int");

                    b.HasKey("CommentId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProductId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Customer", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("CustomerCode")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<bool?>("Gender")
                        .HasColumnType("bit");

                    b.Property<long>("MaxCodeNumber")
                        .HasColumnType("bigint");

                    b.HasKey("CustomerId");

                    b.HasIndex("CustomerCode")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Employee", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployeeCode")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<float>("Gender")
                        .HasColumnType("real");

                    b.HasKey("EmployeeId");

                    b.HasIndex("EmployeeCode")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Favourite", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ProductId")
                        .HasColumnType("char(36)");

                    b.HasKey("CustomerId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("Favourites");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Image", b =>
                {
                    b.Property<string>("ImageId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ImageName")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.HasKey("ImageId");

                    b.HasIndex("ProductId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Order", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateDelivery")
                        .HasColumnType("datetime2");

                    b.Property<long>("MaxCodeNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("OrderCode")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("TransportFee")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("OrderCode")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.OrderDetail", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("OrderId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Product", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<string>("BrandId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<string>("Decription")
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<long>("MaxCodeNumber")
                        .HasColumnType("bigint");

                    b.Property<double?>("PercentDiscount")
                        .HasColumnType("float");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductCode")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("ProductSlug")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductCode")
                        .IsUnique();

                    b.HasIndex("ProductSlug")
                        .IsUnique();

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens", (string)null);
                });

            modelBuilder.Entity("Book_Ecommerce.Models.AppUser", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Customer", "Customer")
                        .WithOne("User")
                        .HasForeignKey("Book_Ecommerce.Models.AppUser", "CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Book_Ecommerce.Models.Employee", "Employee")
                        .WithOne("User")
                        .HasForeignKey("Book_Ecommerce.Models.AppUser", "EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Comment", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Customer", "Customer")
                        .WithMany("Comments")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Book_Ecommerce.Models.Product", "Product")
                        .WithMany("Comments")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Favourite", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Customer", "Customer")
                        .WithMany("Favourites")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Book_Ecommerce.Models.Product", "Product")
                        .WithMany("Favourites")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Image", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Order", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.OrderDetail", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Book_Ecommerce.Models.Product", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Product", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.Brand", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Book_Ecommerce.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Book_Ecommerce.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Book_Ecommerce.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Brand", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Customer", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Favourites");

                    b.Navigation("Orders");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Employee", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("Book_Ecommerce.Models.Product", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Favourites");

                    b.Navigation("Images");

                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
