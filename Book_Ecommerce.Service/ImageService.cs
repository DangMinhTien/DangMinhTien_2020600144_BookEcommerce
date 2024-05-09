using Book_Ecommerce.Data.Abstract;
using Book_Ecommerce.Domain.Entities;
using Book_Ecommerce.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book_Ecommerce.Service
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Image>> GetImageByProductAsync(string productId)
        {
            return await _unitOfWork.ImageRepository.GetDataAsync(i => i.ProductId ==  productId);
        }
        public async Task<Image?> GetSingleByConditionAsync(Expression<Func<Image, bool>> expression)
        {
            return await _unitOfWork.ImageRepository.GetSingleByConditionAsync(expression);
        }
        public async Task RemoveAsync(Image image)
        {
            _unitOfWork.ImageRepository.Remove(image);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AddAsync(Image image)
        {
            await _unitOfWork.ImageRepository.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
