using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels;
using Book_Ecommerce.Service.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<CartItemVM>> GetByCustomerToViewAsync(string customerId)
        {
            var cartItems = await _unitOfWork.CartItemRepository.Table()
                                                        .Include(c => c.Product)
                                                        .ThenInclude(p => p.Images)
                                                        .Where(c => c.CustomerId == customerId)
                                                        .ToListAsync();
            var cartItemVM = cartItems.Select(c => new CartItemVM
            {
                ProductId = c.ProductId,
                ProductName = c.Product.ProductName,
                ProductSlug = c.Product.ProductSlug,
                Price = (c.Product.PercentDiscount == null || c.Product.PercentDiscount == 0) ? c.Product.Price
                                : c.Product.Price - (c.Product.Price * (decimal)c.Product.PercentDiscount / 100),
                Image = c.Product.Images.FirstOrDefault()?.Url ?? "",
                ProductCode = c.Product.ProductCode,
                Quantity = c.Quantity
            }).ToList();
            return cartItemVM;
        }
        public async Task<CartItem?> GetSingleByConditionAsync(Expression<Func<CartItem, bool>> expression)
        {
            return await _unitOfWork.CartItemRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<CartItem>> GetDataAsync(Expression<Func<CartItem, bool>>? expression = null)
        {
            return await _unitOfWork.CartItemRepository.GetDataAsync(expression);
        }
        public async Task AddAsync(CartItem cartItem)
        {
            await _unitOfWork.CartItemRepository.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(CartItem cartItem)
        {
            _unitOfWork.CartItemRepository.Remove(cartItem);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveRangeAsync(IEnumerable<CartItem> cartItems)
        {
            _unitOfWork.CartItemRepository.RemoveRange(cartItems);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(CartItem cartItem)
        {
            _unitOfWork.CartItemRepository.Update(cartItem);
            await _unitOfWork.SaveChangesAsync();
        }
        public IQueryable<CartItem> Table()
        {
            return _unitOfWork.CartItemRepository.Table();
        }
    }
}
