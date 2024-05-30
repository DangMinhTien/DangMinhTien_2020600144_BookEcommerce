using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetByAuthorToViewAsync(string authorId, string authorSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE);
        Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetByBrandToViewAsync(string brandId, string brandSlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE);
        Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetByCategoryToViewAsync(string categoryId, string categorySlug, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE);
        Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)> 
            GetToViewAsync(string? search, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE);
        Task<(IEnumerable<ProductVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search, int page = 1, int pagesize = 12);
        Task<ProductVM?> GetDetailToViewAsync(string productSlug);
        Task SaveChangesAsync();
        IQueryable<Product> Table();
        Task<Product?> GetSingleByConditionAsync(Expression<Func<Product, bool>> expression);
        Product? GetSingleByCondition(Expression<Func<Product, bool>> expression);
        Task AddAsync(Product product, IEnumerable<Image>? images = null, IEnumerable<CategoryProducts>? categoryProducts = null, IEnumerable<AuthorProduct>? authorProducts = null);
        Task<ProductVM?> GetDetailToViewManageAsync(string productCode);
        Task UpdateAsync(Product product);
        Task RemoveAsync(Product product);
        Task<IEnumerable<ProductVM>> GetTopSelling();
    }
}
