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
        Task<District?> GetSingleDistrictByConditionAsync(Expression<Func<District, bool>> expression);
        Task<Province?> GetSingleProvinceByConditionAsync(Expression<Func<Province, bool>> expression);
        Task<Ward?> GetSingleWardByConditionAsync(Expression<Func<Ward, bool>> expression);
    }
}
