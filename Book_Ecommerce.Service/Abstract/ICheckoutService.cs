using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface ICheckoutService
    {
        Task AddOrderAsync(Order order, IEnumerable<OrderDetail>? orders = null);
        Task AddRangeOrderDetaiAsync(IEnumerable<OrderDetail> orderDetails);
        Task<Order?> GetSingleOrderByConditionAsync(Expression<Func<Order, bool>> expression);
        IQueryable<OrderDetail> OrderDetailTable();
        IQueryable<Order> OrderTable();
        Task RemoveOrderAsync(Order order);
    }
}
