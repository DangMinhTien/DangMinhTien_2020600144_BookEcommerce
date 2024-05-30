using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.EmployeeViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IEmployeeService
    {
        Task<Employee?> GetSingleByCondionAsync(Expression<Func<Employee, bool>> expression);
        Task<(IEnumerable<EmployeeVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Employee employee);
        IQueryable<Employee> Table();
        Task UpdateAsync(Employee employee);
    }
}
