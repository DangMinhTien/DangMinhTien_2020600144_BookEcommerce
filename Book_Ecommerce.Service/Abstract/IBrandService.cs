using Book_Ecommerce.Domain.Entities;
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
        Task<IEnumerable<Brand>> GetDataAsync(Expression<Func<Brand, bool>>? expression = null);
        Task<Brand?> GetSingleByConditionAsync(Expression<Func<Brand, bool>> expression);
        Task<IEnumerable<Brand>> GetToViewComponentAsync();
    }
}
