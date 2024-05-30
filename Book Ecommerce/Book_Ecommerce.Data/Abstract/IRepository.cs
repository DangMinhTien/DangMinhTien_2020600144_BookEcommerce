using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Data.Abstract
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetDataAsync(Expression<Func<T, bool>>? expression = null);
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetSingleByConditionAsync(Expression<Func<T, bool>> expression);
        T? GetSingleByCondition(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        IQueryable<T> Table();
    }
}
