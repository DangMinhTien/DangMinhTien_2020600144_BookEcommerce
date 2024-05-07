using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Order> OrderTable()
        {
            return _unitOfWork.OrderRepository.Table();
        }
        public IQueryable<OrderDetail> OrderDetailTable()
        {
            return _unitOfWork.OrderDetailRepository.Table();
        }
        public async Task<Order?> GetSingleOrderByConditionAsync(Expression<Func<Order, bool>> expression)
        {
            return await _unitOfWork.OrderRepository.GetSingleByConditionAsync(expression);
        }
        public async Task RemoveOrderAsync(Order order)
        {
            _unitOfWork.OrderRepository.Remove(order);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddOrderAsync(Order order, IEnumerable<OrderDetail>? orderDetails = null)
        {
            await _unitOfWork.OrderRepository.AddAsync(order);
            if(orderDetails != null && orderDetails.Count() > 0)
            {
                await _unitOfWork.OrderDetailRepository.AddRangeAsync(orderDetails);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddRangeOrderDetaiAsync(IEnumerable<OrderDetail> orderDetails)
        {
            await _unitOfWork.OrderDetailRepository.AddRangeAsync(orderDetails);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
