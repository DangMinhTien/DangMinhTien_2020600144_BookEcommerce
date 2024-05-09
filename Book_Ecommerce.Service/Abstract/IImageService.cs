using Book_Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IImageService
    {
        Task AddAsync(Image image);
        Task<IEnumerable<Image>> GetImageByProductAsync(string productId);
        Task<Image?> GetSingleByConditionAsync(Expression<Func<Image, bool>> expression);
        Task RemoveAsync(Image image);
    }
}
