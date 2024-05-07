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
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Author?> GetSingleByConditionAsync(Expression<Func<Author, bool>> expression)
        {
            return await _unitOfWork.AuthorRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Author>> GetDataAsync(Expression<Func<Author, bool>>? expression = null)
        {
            return await _unitOfWork.AuthorRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Author>> GetToViewComponentAsync()
        {
            return await _unitOfWork.AuthorRepository
                                    .Table()
                                    .Include(a => a.AuthorProducts)
                                    .ToListAsync();
        }
    }
}
