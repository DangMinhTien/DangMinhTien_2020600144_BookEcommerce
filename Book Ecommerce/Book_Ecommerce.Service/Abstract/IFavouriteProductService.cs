using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IFavouriteProductService
    {
        Task AddAsync(FavouriteProduct favouriteProduct);
        Task<FavouriteProduct?> GetSingleByConditionAsync(Expression<Func<FavouriteProduct, bool>> expression);
        Task RemoveAsync(FavouriteProduct favouriteProduct);
        IQueryable<FavouriteProduct> Table();
    }
}
