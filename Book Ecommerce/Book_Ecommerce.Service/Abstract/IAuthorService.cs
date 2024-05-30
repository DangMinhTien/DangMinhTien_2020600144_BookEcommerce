using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Domain.Models;
using Book_Ecommerce.Domain.ViewModels.AuthorViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service.Abstract
{
    public interface IAuthorService
    {
        Task AddAsync(Author author);
        Task<IEnumerable<Author>> GetDataAsync(Expression<Func<Author, bool>>? expression = null);
        Task<Author?> GetSingleByConditionAsync(Expression<Func<Author, bool>> expression);
        Task<IEnumerable<Author>> GetToViewComponentAsync();
        Task<(IEnumerable<AuthorVM>, PagingModel, IEnumerable<PageSizeModel>)> GetToViewManageAsync(string? search = null, int page = 1, int pagesize = 20);
        Task RemoveAsync(Author author);
        IQueryable<Author> Table();
        Task UpdateAsync(Author author);
    }
}
