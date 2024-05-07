using Book_Ecommerce.Data;
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
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Brand?> GetSingleByConditionAsync(Expression<Func<Brand, bool>> expression)
        {
            return await _unitOfWork.BrandRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Brand>> GetDataAsync(Expression<Func<Brand, bool>>? expression = null)
        {
            return await _unitOfWork.BrandRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Brand>> GetToViewComponentAsync()
        {
            return await _unitOfWork.BrandRepository
                                    .Table()
                                    .Include(b => b.Products)
                                    .ToListAsync();
        }
    }
}
