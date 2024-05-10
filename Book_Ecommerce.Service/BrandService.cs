using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.BrandViewModel;
using Book_Ecommerce.Domain.ViewModels.CategoryViewModel;
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
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public BrandService(IUnitOfWork unitOfWork, IUrlHelperFactory urlHelperFactory, IActionContextAccessor action)
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
        public async Task<Brand?> GetSingleByConditionAsync(Expression<Func<Brand, bool>> expression)
        {
            return await _unitOfWork.BrandRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Brand>> GetDataAsync(Expression<Func<Brand, bool>>? expression = null)
        {
            return await _unitOfWork.BrandRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Brand>> GetToViewComponentAsync()
        {
            return await _unitOfWork.BrandRepository
                                    .Table()
                                    .Include(b => b.Products)
                                    .ToListAsync();
        }
        public async Task<(IEnumerable<BrandVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.BrandRepository.Table()
                                                    .Include(b => b.Products)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.BrandName.Contains(search));
            }
            var brands = await query.OrderBy(b => b.CodeNumber).ToListAsync();
            #region bắt đầu phân trang
            var totalItem = brands.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var brandVMs = brands.Select(b => new BrandVM
            {
                BrandId = b.BrandId,
                BrandName  = b.BrandName,
                BrandCode = b.BrandCode,
                BrandSlug = b.BrandSlug,
                IsActive = true,
                SumProduct = b.Products.Count(),
                ImageName = b.ImageName,
                UrlImage = b.UrlImage,
            }).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Brands", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Brands", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Brands", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Brands", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (brandVMs, pagingModel, lstPageSize);
        }
        public IQueryable<Brand> Table()
        {
            return _unitOfWork.BrandRepository.Table();
        }
        public async Task AddAsync(Brand brand)
        {
            await _unitOfWork.BrandRepository.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(Brand brand)
        {
            _unitOfWork.BrandRepository.Update(brand);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Brand brand)
        {
            _unitOfWork.BrandRepository.Remove(brand);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
