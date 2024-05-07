using Book_Ecommerce.Domain.Entities;
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
        Task<IEnumerable<Category>> GetDataAsync(Expression<Func<Category, bool>>? expression = null);
        Task<Category?> GetSingleByConditionAsync(Expression<Func<Category, bool>> expression);
        Task<IEnumerable<Category>> GetToViewComponentAsync();
    }
}
