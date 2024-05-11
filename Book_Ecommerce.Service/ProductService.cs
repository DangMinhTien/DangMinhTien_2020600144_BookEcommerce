using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Linq.Expressions;
using Book_Ecommerce.Data;

namespace Book_Ecommerce.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public ProductService(IUnitOfWork unitOfWork, IUrlHelperFactory urlHelperFactory, IActionContextAccessor action)
        {
            _unitOfWork = unitOfWork;
            _urlHelper = urlHelperFactory.GetUrlHelper(action.ActionContext ?? new ActionContext());
            lstPageSize = new List<PageSizeModel>
            {
                new PageSizeModel
                {
                    Size = MyAppSetting.PAGE_SIZE,
                },
                new PageSizeModel
                {
                    Size = 30,
                },
                new PageSizeModel
                {
                    Size = 50,
                }
            };
        }
        public IQueryable<Product> Table()
        {
            return _unitOfWork.ProductRepository.Table();
        }
        public async Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)> 
            GetToViewAsync(string? search, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.ProductRepository.Table()
                                            .Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .ThenInclude(c => c.Category)
                                            .Include(p => p.Brand)
                                            .Where(p => p.IsActive)
                                            .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }
            var products = await query.ToListAsync();
            #region bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var productVMs = products.Select(p => new ProductVM
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
                Brand = p.Brand,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Products", new { page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Products", new { page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Products", new { page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Products", new { page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (productVMs, pagingModel, lstPageSize);
        }
        public async Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.ProductRepository.Table()
                                                    .Include(p => p.Images)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }
            query = query.OrderBy(p => p.CodeNumber);

            #region Bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var products = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var productVMs = products.Select(p => new ProductVM
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                Quantity = p.Quantity,
                Price = p.Price,
                PercentDiscount = p.PercentDiscount,
                IsActive = p.IsActive,
                Decription = p.Description,
                Images = p.Images
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Products", new { page = p, pagesize = pagesize, area = "Admin" }) ?? "" :
                                        _urlHelper.Action("Index", "Products", new { page = p, search = search, pagesize = pagesize, area = "Admin" }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Products", new { page = page, pagesize = psItem.Size, area = "Admin" }) ?? "" :
                                        _urlHelper.Action("Index", "Products", new { page = page, search = search, pagesize = psItem.Size, area = "Admin" }) ?? "";
            }
            #endregion kết thúc phân trang
            return (productVMs, pagingModel, lstPageSize);
        }
        public async Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)> 
            GetByCategoryToViewAsync(string categoryId, string categorySlug,int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var products = await _unitOfWork.ProductRepository.Table()
                                            .Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .ThenInclude(c => c.Category)
                                            .Include(p => p.Brand)
                                            .Where(p => p.IsActive && p.CategoryProducts.Any(c => c.CategoryId == categoryId))
                                            .ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var productVMs = products.Select(p => new ProductVM
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
                Categories = p.CategoryProducts.Select(c => c.Category).ToList(),
                Brand = p.Brand,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => _urlHelper.Action("GetByCategory", "Products", new { page = p, categorySlug = categorySlug, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = _urlHelper.Action("GetByCategory", "Products", new { page = page, categorySlug = categorySlug, pagesize = psItem.Size }) ?? "";
            }
            #endregion kết thúc phân trang
            return (productVMs, pagingModel, lstPageSize);
        }
        public async Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetByBrandToViewAsync(string brandId, string brandSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var products = await _unitOfWork.ProductRepository.Table()
                                            .Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .ThenInclude(c => c.Category)
                                            .Include(p => p.Brand)
                                            .Where(p => p.IsActive && p.BrandId == brandId)
                                            .ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var productVMs = products.Select(p => new ProductVM
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
                Categories = p.CategoryProducts.Select(c => c.Category).ToList(),
                Brand = p.Brand,
                Images = p.Images
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => _urlHelper.Action("GetbyBrand", "Products", new { page = p, brandSlug = brandSlug, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = _urlHelper.Action("GetbyBrand", "Products", new { page = page, brandSlug = brandSlug, pagesize = psItem.Size }) ?? "";
            }
            #endregion kết thúc phân trang
            return (productVMs, pagingModel, lstPageSize);
        }
        public async Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)> 
            GetByAuthorToViewAsync(string authorId, string authorSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var products = await _unitOfWork.ProductRepository.Table()
                                            .Include(p => p.Images)
                                            .Include(p => p.AuthorProducts)
                                            .ThenInclude(ap => ap.Author)
                                            .Where(p => p.IsActive && p.AuthorProducts.Any(ap => ap.AuthorId == authorId))
                                            .ToListAsync();
            #region Bắt đầu phân trang
            var totalItem = products.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var productVMs = products.Select(p => new ProductVM
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
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => _urlHelper.Action("GetByAuthor", "Products", new { page = p, authorSlug = authorSlug, pagesize = pagesize }) ?? ""
            };
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = _urlHelper.Action("GetByAuthor", "Products", new { page = page, authorSlug = authorSlug, pagesize = psItem.Size }) ?? "";
            }
            #endregion kết thúc phân trang
            return (productVMs, pagingModel, lstPageSize);
        }
        public async Task<ProductVM?> GetDetailToViewAsync(string productSlug)
        {
            var product = await _unitOfWork.ProductRepository.Table()
                                           .Include(p => p.Images)
                                           .Include(p => p.Brand)
                                           .Include(p => p.CategoryProducts)
                                           .ThenInclude(p => p.Category)
                                           .Include(p => p.AuthorProducts)
                                           .ThenInclude(p => p.Author)
                                           .Include(p => p.Comments)
                                           .ThenInclude(c => c.Customer)
                                           .FirstOrDefaultAsync(p => p.ProductSlug == productSlug);
            if (product == null)
                return null;
            var categoryIds = product.CategoryProducts.Select(cp => cp.CategoryId).ToList();
            var products = await _unitOfWork.ProductRepository.Table()
                                            .Include(p => p.Images)
                                            .Include(p => p.CategoryProducts)
                                            .Where(p => p.CategoryProducts.Any(cp => categoryIds.Contains(cp.CategoryId))
                                                && p.ProductId != product.ProductId)
                                            .ToListAsync();
            var productVM = new ProductVM
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                ProductSlug = product.ProductSlug,
                Images = product.Images,
                PriceAfterDiscount = (product.PercentDiscount > 0) ? product.Price - (product.Price * ((decimal)product.PercentDiscount / 100)) : product.Price,
                PercentDiscount = product.PercentDiscount,
                Price = product.Price,
                Decription = product.Description,
                Quantity = product.Quantity,
                Categories = product.CategoryProducts.Select(p => p.Category).ToList(),
                Authors = product.AuthorProducts.Select(p => p.Author).ToList(),
                Brand = product.Brand,
                Comments = product.Comments,
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
                }).ToList()
            };
            return productVM;
        }
        public async Task<ProductVM?> GetDetailToViewManageAsync(string productCode)
        {
            var product = await _unitOfWork.ProductRepository.Table()
                                           .Include(p => p.Brand)
                                           .Include(p => p.CategoryProducts)
                                           .ThenInclude(p => p.Category)
                                           .Include(p => p.AuthorProducts)
                                           .ThenInclude(p => p.Author)
                                           .FirstOrDefaultAsync(p => p.ProductCode == productCode);
            if (product == null)
                return null;
            var productVM = new ProductVM
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                ProductSlug = product.ProductSlug,
                PercentDiscount = product.PercentDiscount,
                Price = product.Price,
                Decription = product.Description,
                Quantity = product.Quantity,
                IsActive = product.IsActive,
                Categories = product.CategoryProducts.Select(p => p.Category).ToList(),
                Authors = product.AuthorProducts.Select(p => p.Author).ToList(),
                Brand = product.Brand
            };
            return productVM;
        }
        public async Task<Product?> GetSingleByConditionAsync(Expression<Func<Product, bool>> expression)
        {
            return await _unitOfWork.ProductRepository.GetSingleByConditionAsync(expression);
        }
        public Product? GetSingleByCondition(Expression<Func<Product, bool>> expression)
        {
            return _unitOfWork.ProductRepository.GetSingleByCondition(expression);
        }
        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddAsync(Product product, IEnumerable<Image>? images = null, 
            IEnumerable<CategoryProducts>? categoryProducts = null, IEnumerable<AuthorProduct>? authorProducts = null)
        {
            await _unitOfWork.ProductRepository.AddAsync(product);
            if(images != null && images.Count() > 0)
            {
                await _unitOfWork.ImageRepository.AddRangeAsync(images);
            }
            if (categoryProducts != null && categoryProducts.Count() > 0)
            {
                await _unitOfWork.CategoryProductRepository.AddRangeAsync(categoryProducts);
            }
            if (authorProducts != null && authorProducts.Count() > 0)
            {
                await _unitOfWork.AuthorProductRepository.AddRangeAsync(authorProducts);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Product product)
        {
            _unitOfWork.ProductRepository.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
