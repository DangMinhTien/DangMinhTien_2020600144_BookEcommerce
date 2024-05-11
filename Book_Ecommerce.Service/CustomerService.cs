using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.CustomerViewModel;
using Book_Ecommerce.Domain.ViewModels.EmployeeViewModel;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public CustomerService(IUnitOfWork unitOfWork, 
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor action)
        {
            _unitOfWork = unitOfWork;
            _urlHelper = urlHelperFactory.GetUrlHelper(action.ActionContext ?? new ActionContext());
            lstPageSize = new List<PageSizeModel>
            {
                new PageSizeModel
                {
                    Size = MyAppSetting.PAGE_SIZE,
                },
                new PageSizeModel
                {
                    Size = 30,
                },
                new PageSizeModel
                {
                    Size = 50,
                }
            };
        }
        public async Task<(IEnumerable<CustomerVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.CustomerRepository.Table()
                                                    .Include(e => e.User)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FullName.Contains(search));
            }
            query = query.OrderBy(c => c.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var customers = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var customerVMs = customers.Select(c => new CustomerVM
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Gender = c.Gender,
                Address = c.Address,
                CustomerCode = c.CustomerCode,
                DateOfBirth = c.DateOfBirth,
                User = c.User
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Customers", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Customers", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Customers", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Customers", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (customerVMs, pagingModel, lstPageSize);
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
        public async Task RemoveAsync(Customer customer)
        {
            _unitOfWork.CustomerRepository.Remove(customer);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
