using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface ICartService
    {
        Task AddAsync(CartItem cartItem);
        Task<IEnumerable<CartItemVM>> GetByCustomerToViewAsync(string customerId);
        Task<IEnumerable<CartItem>> GetDataAsync(Expression<Func<CartItem, bool>>? expression = null);
        Task<CartItem?> GetSingleByConditionAsync(Expression<Func<CartItem, bool>> expression);
        Task RemoveAsync(CartItem cartItem);
        Task RemoveRangeAsync(IEnumerable<CartItem> cartItems);
        IQueryable<CartItem> Table();
        Task UpdateAsync(CartItem cartItem);
    }
}
