using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.BrandViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IBrandService
    {
        Task AddAsync(Brand brand);
        Task<IEnumerable<Brand>> GetDataAsync(Expression<Func<Brand, bool>>? expression = null);
        Task<Brand?> GetSingleByConditionAsync(Expression<Func<Brand, bool>> expression);
        Task<IEnumerable<Brand>> GetToViewComponentAsync();
        Task<(IEnumerable<BrandVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Brand brand);
        IQueryable<Brand> Table();
        Task UpdateAsync(Brand brand);
    }
}
