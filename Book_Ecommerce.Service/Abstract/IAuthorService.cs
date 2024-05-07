using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetDataAsync(Expression<Func<Author, bool>>? expression = null);
        Task<Author?> GetSingleByConditionAsync(Expression<Func<Author, bool>> expression);
        Task<IEnumerable<Author>> GetToViewComponentAsync();
    }
}
