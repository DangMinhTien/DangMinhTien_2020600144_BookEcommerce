using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.OrderViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IOrderService
    {
        Task<dynamic?> GetOrderDetailToView(string orderId);
        Task<Order?> GetSingleByConditionAsync(Expression<Func<Order, bool>> expression);
        Task<(IEnumerable<OrderVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task<(IEnumerable<OrderVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewMyOrderAsync(string customerId, string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Order order);
        IQueryable<Order> Table();
        IQueryable<OrderDetail> TableOrderDetail();
        Task UpdateAsync(Order order);
    }
}
