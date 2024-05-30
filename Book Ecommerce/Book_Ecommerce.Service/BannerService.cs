using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.BannerViewModel;
using Book_Ecommerce.Domain.ViewModels.BrandViewModel;
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
    public class BannerService : IBannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public BannerService(IUnitOfWork unitOfWork, 
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
        public async Task<(IEnumerable<BannerVM>, PagingModel, IEnumerable<PageSizeModel>)>
            GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.BannerRepository.Table()
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search));
            }
            query = query.OrderByDescending(b => b.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var banners = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var bannerVMs = banners.Select(b => new BannerVM
            {
                BannerId = b.BannerId,
                BannerCode = b.BannerCode,
                Title = b.Title,
                Content = b.Content,
                ImageName = b.ImageName,
                UrlImage = b.UrlImage,
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Banners", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Banners", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Banners", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Banners", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (bannerVMs, pagingModel, lstPageSize);
        }
        public async Task<Banner?> GetSingleByConditionAsync(Expression<Func<Banner, bool>> expression)
        {
            return await _unitOfWork.BannerRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Banner>> GetDataAsync(Expression<Func<Banner, bool>>? expression = null)
        {
            return await _unitOfWork.BannerRepository.GetDataAsync(expression);
        }
        public IQueryable<Banner> Table()
        {
            return _unitOfWork.BannerRepository.Table();
        }
        public async Task AddAsync(Banner banner)
        {
            await _unitOfWork.BannerRepository.AddAsync(banner);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(Banner banner)
        {
            _unitOfWork.BannerRepository.Update(banner);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Banner banner)
        {
            _unitOfWork.BannerRepository.Remove(banner);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
