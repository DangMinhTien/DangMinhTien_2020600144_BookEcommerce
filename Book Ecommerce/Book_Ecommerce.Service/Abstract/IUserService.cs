using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.ViewModels.EmployeeViewModel;
using Book_Ecommerce.Domain.ViewModels.AccountViewModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IUserService
    {
        Task<AppUser?> GetSingleByConditionAsync(Expression<Func<AppUser, bool>> expression);
        IQueryable<AppUser> Table();
        Task<(IdentityResult, AppUser, Customer)> RegisterCustomerAccountAsync(RegisterVM registerVM);
        Task<(IdentityResult, AppUser, Employee)> RegisterEmployeeAccountAsync(InputEmployee inputEmployee);
    }
}
