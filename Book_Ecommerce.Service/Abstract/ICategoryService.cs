using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface ICategoryService
    {
        Task AddAsync(Category category);
        Task<IEnumerable<Category>> GetDataAsync(Expression<Func<Category, bool>>? expression = null);
        Task<Category?> GetSingleByConditionAsync(Expression<Func<Category, bool>> expression);
        Task<IEnumerable<Category>> GetToViewComponentAsync();
        Task<(IEnumerable<CategoryVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Category category);
        IQueryable<Category> Table();
        Task UpdateAsync(Category category);
    }
}
