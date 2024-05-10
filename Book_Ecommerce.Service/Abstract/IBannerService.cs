using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.BannerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IBannerService
    {
        Task AddAsync(Banner banner);
        Task<Banner?> GetSingleByConditionAsync(Expression<Func<Banner, bool>> expression);
        Task<(IEnumerable<BannerVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Banner banner);
        IQueryable<Banner> Table();
        Task UpdateAsync(Banner banner);
    }
}
