using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Book_Ecommerce.Service
{
    public class FavouriteProductService : IFavouriteProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavouriteProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<FavouriteProduct?> 
            GetSingleByConditionAsync(Expression<Func<FavouriteProduct, bool>> expression)
        {
            return await _unitOfWork.FavouriteProductRepository.GetSingleByConditionAsync(expression);
        }
        public IQueryable<FavouriteProduct> Table() 
        {
            return _unitOfWork.FavouriteProductRepository.Table();
        }
        public async Task AddAsync(FavouriteProduct favouriteProduct)
        {
            await _unitOfWork.FavouriteProductRepository.AddAsync(favouriteProduct);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(FavouriteProduct favouriteProduct)
        {
            _unitOfWork.FavouriteProductRepository.Remove(favouriteProduct);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
