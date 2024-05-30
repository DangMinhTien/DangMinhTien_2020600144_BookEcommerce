using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IProvinceService
    {
        Task<IEnumerable<District>> GetDataDistrictAsync(Expression<Func<District, bool>>? expression = null);
        Task<IEnumerable<Province>> GetDataProvinceAsync(Expression<Func<Province, bool>>? expression = null);
        Task<IEnumerable<Ward>> GetDataWardAsync(Expression<Func<Ward, bool>>? expression = null);
        Task<District?> GetSingleDistrictByConditionAsync(Expression<Func<District, bool>> expression);
        Task<Province?> GetSingleProvinceByConditionAsync(Expression<Func<Province, bool>> expression);
        Task<Ward?> GetSingleWardByConditionAsync(Expression<Func<Ward, bool>> expression);
    }
}
