using Book_Ecommerce.Data;
using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.MySettings;
using Book_Ecommerce.Domain.ViewModels.AuthorViewModel;
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
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelper _urlHelper;
        private List<PageSizeModel> lstPageSize;

        public AuthorService(IUnitOfWork unitOfWork, IUrlHelperFactory urlHelperFactory, IActionContextAccessor action)
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
        public async Task<Author?> GetSingleByConditionAsync(Expression<Func<Author, bool>> expression)
        {
            return await _unitOfWork.AuthorRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Author>> GetDataAsync(Expression<Func<Author, bool>>? expression = null)
        {
            return await _unitOfWork.AuthorRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Author>> GetToViewComponentAsync()
        {
            return await _unitOfWork.AuthorRepository
                                    .Table()
                                    .Include(a => a.AuthorProducts)
                                    .ToListAsync();
        }
        public async Task<(IEnumerable<AuthorVM>, PagingModel, IEnumerable<PageSizeModel>)>
           GetToViewManageAsync(string? search = null, int page = 1, int pagesize = MyAppSetting.PAGE_SIZE)
        {
            var query = _unitOfWork.AuthorRepository.Table()
                                                    .Include(c => c.AuthorProducts)
                                                    .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.AuthorName.Contains(search));
            }
            query = query.OrderBy(a => a.CodeNumber);
            #region bắt đầu phân trang
            var totalItem = query.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pagesize);
            if (page > totalPage)
                page = totalPage;
            if (page < 1)
                page = 1;
            var authors = await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();
            var authorVMs = authors.Select(a => new AuthorVM
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                AuthorSlug = a.AuthorSlug,
                IsActive = true,
                AuthorCode = a.AuthorCode,
                Information = a.Information,
                SumProduct = a.AuthorProducts.Count(),
            }).ToList();
            var pagingModel = new PagingModel
            {
                currentpage = page,
                countpages = totalPage,
                generateUrl = (p) => string.IsNullOrEmpty(search) ?
                                        _urlHelper.Action("Index", "Authors", new { area = "Admin", page = p, pagesize = pagesize }) ?? "" :
                                        _urlHelper.Action("Index", "Authors", new { area = "Admin", page = p, search = search, pagesize = pagesize }) ?? ""
            };
            #endregion kết thúc phân trang
            foreach (var psItem in lstPageSize)
            {
                psItem.IsActive = psItem.Size == pagesize ? true : false;
                psItem.Url = string.IsNullOrEmpty(search) ?
                _urlHelper.Action("Index", "Authors", new { area = "Admin", page = page, pagesize = psItem.Size }) ?? "" :
                                        _urlHelper.Action("Index", "Authors", new { area = "Admin", page = page, search = search, pagesize = psItem.Size }) ?? "";
            }
            return (authorVMs, pagingModel, lstPageSize);
        }
        public IQueryable<Author> Table()
        {
            return _unitOfWork.AuthorRepository.Table();
        }
        public async Task AddAsync(Author author)
        {
            await _unitOfWork.AuthorRepository.AddAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAsync(Author author)
        {
            _unitOfWork.AuthorRepository.Update(author);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAsync(Author author)
        {
            _unitOfWork.AuthorRepository.Remove(author);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
