using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Book_Ecommerce.Domain.ViewModels.BannerViewModel;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Domain.ViewModels.EmployeeViewModel;
using Microsoft.AspNetCore.Identity;
using Book_Ecommerce.Domain.Entities;
using System.Linq.Expressions;

namespace Book_Ecommerce.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeService(IUnitOfWork unitOfWork,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor action,
            UserManager<AppUser> userManager)
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
            _userManager = userManager;
        }
        public async Task<(IEnumerable<EmployeeVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.EmployeeRepository.Table()
                                                    .Include(e => e.User)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.FullName.Contains(search));
            }
            query = query.OrderBy(e => e.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var employees = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var employeeVMs = employees.Select(e => new EmployeeVM
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Gender = e.Gender,
                Address = e.Address,
                EmployeeCode = e.EmployeeCode,
                DateOfBirth = e.DateOfBirth,
                User = e.User
            }).ToList();
            foreach(var employeeVM in employeeVMs)
            {
                employeeVM.RoleNames = (await _userManager.GetRolesAsync(employeeVM.User)).ToList();
            }
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Employees", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Employees", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Employees", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Employees", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (employeeVMs, pagingModel, lstPageSize);
        }
        public IQueryable<Employee> Table()
        {
            return _unitOfWork.EmployeeRepository.Table();
        }
        public async Task<Employee?> GetSingleByCondionAsync(Expression<Func<Employee, bool>> expression)
        {
            return await _unitOfWork.EmployeeRepository.GetSingleByConditionAsync(expression);
        }
        public async Task UpdateAsync(Employee employee)
        {
            _unitOfWork.EmployeeRepository.Update(employee);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Employee employee)
        {
            _unitOfWork.EmployeeRepository.Remove(employee);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
