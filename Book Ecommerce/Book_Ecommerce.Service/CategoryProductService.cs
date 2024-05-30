using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class CategoryProductService : ICategoryProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddRangeAsync(IEnumerable<CategoryProducts> categoryProducts)
        {
            await _unitOfWork.CategoryProductRepository.AddRangeAsync(categoryProducts);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveRangeAsync(IEnumerable<CategoryProducts> categoryProducts)
        {
            _unitOfWork.CategoryProductRepository.RemoveRange(categoryProducts);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
