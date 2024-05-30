using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.CustomerViewModel;
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
        Task<(IEnumerable<CustomerVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Customer customer);
        Task SaveChangesAsync();
        IQueryable<Customer> Table();
        Task UpdateAsync(Customer customer);
    }
}
