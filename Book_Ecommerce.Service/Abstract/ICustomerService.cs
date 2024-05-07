using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface ICustomerService
    {
        Task AddAsync(Customer customer);
        Task<Customer?> GetSingleByConditionAsync(Expression<Func<Customer, bool>> expression);
        Task SaveChangesAsync();
        IQueryable<Customer> Table();
    }
}
