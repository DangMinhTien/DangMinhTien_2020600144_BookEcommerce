using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
using Book_Ecommerce.Domain.ViewModels.ProductViewModel;
using Book_Ecommerce.Service.Abstract;
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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public CategoryService(IUnitOfWork unitOfWork, IUrlHelperFactory urlHelperFactory, IActionContextAccessor action)
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
        public async Task<Category?> GetSingleByConditionAsync(Expression<Func<Category, bool>> expression)
        {
            return await _unitOfWork.CategoryRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Category>> GetDataAsync(Expression<Func<Category, bool>>? expression = null)
        {
            return await _unitOfWork.CategoryRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Category>> GetToViewComponentAsync()
        {
            return await _unitOfWork.CategoryRepository
                                    .Table()
                                    .Include(c => c.CategoryProducts)
                                    .ToListAsync();
        }
        public async Task<(IEnumerable<CategoryVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.CategoryRepository.Table()
                                                    .Include(c => c.CategoryProducts)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CategoryName.Contains(search));
            }
            query =  query.OrderByDescending(c => c.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var categories = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var categoryVMs = categories.Select(c => new CategoryVM
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategorySlug = c.CategorySlug,
                IsActive = true,
                CategoryCode = c.CategoryCode,
                Description = c.Description,
                SumProduct = c.CategoryProducts.Count(),
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Categories", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Categories", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Categories", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Categories", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (categoryVMs, pagingModel, lstPageSize);
        }
        public IQueryable<Category> Table()
        {
            return _unitOfWork.CategoryRepository.Table();
        }
        public async Task AddAsync(Category category)
        {
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Category category)
        {
            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync() ;
        }
    }
}
