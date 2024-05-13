using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Comment> Table()
        {
            return _unitOfWork.CommentRepository.Table();
        }
        public async Task AddAsync(Comment comment)
        {
            await _unitOfWork.CommentRepository.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
