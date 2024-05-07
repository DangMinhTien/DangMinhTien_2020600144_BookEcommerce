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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Customer> Table()
        {
            return _unitOfWork.CustomerRepository.Table();
        }
        public async Task<Customer?> GetSingleByConditionAsync(Expression<Func<Customer, bool>> expression)
        {
            return await _unitOfWork.CustomerRepository.GetSingleByConditionAsync(expression);
        }
        public async Task AddAsync(Customer customer)
        {
            await _unitOfWork.CustomerRepository.AddAsync(customer);
        }
        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
