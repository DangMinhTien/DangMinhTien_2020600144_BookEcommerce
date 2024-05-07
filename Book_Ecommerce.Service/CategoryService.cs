using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Category?> GetSingleByConditionAsync(Expression<Func<Category, bool>> expression)
        {
            return await _unitOfWork.CategoryRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Category>> GetDataAsync(Expression<Func<Category, bool>>? expression = null)
        {
            return await _unitOfWork.CategoryRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Category>> GetToViewComponentAsync()
        {
            return await _unitOfWork.CategoryRepository
                                    .Table()
                                    .Include(c => c.CategoryProducts)
                                    .ToListAsync();
        }
    }
}
