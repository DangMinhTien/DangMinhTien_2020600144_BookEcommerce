using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface ICategoryProductService
    {
        Task AddRangeAsync(IEnumerable<CategoryProducts> categoryProducts);
        Task RemoveRangeAsync(IEnumerable<CategoryProducts> categoryProducts);
    }
}
