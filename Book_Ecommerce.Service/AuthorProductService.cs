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
    public class AuthorProductService : IAuthorProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddRangeAsync(IEnumerable<AuthorProduct> authorProducts)
        {
            await _unitOfWork.AuthorProductRepository.AddRangeAsync(authorProducts);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveRangeAsync(IEnumerable<AuthorProduct> authorProducts)
        {
            _unitOfWork.AuthorProductRepository.RemoveRange(authorProducts);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
