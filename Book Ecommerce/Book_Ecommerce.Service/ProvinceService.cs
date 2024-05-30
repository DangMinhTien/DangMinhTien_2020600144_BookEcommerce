using Book_Ecommerce.Data;
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
    public class ProvinceService : IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProvinceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Province?> GetSingleProvinceByConditionAsync(Expression<Func<Province, bool>> expression)
        {
            return await _unitOfWork.ProvinceRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<District?> GetSingleDistrictByConditionAsync(Expression<Func<District, bool>> expression)
        {
            return await _unitOfWork.DistrictRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<Ward?> GetSingleWardByConditionAsync(Expression<Func<Ward, bool>> expression)
        {
            return await _unitOfWork.WardRepository.GetSingleByConditionAsync(expression);
        }
        public async Task<IEnumerable<Province>> GetDataProvinceAsync(Expression<Func<Province, bool>>? expression = null)
        {
            return await _unitOfWork.ProvinceRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<District>> GetDataDistrictAsync(Expression<Func<District, bool>>? expression = null)
        {
            return await _unitOfWork.DistrictRepository.GetDataAsync(expression);
        }
        public async Task<IEnumerable<Ward>> GetDataWardAsync(Expression<Func<Ward, bool>>? expression = null)
        {
            return await _unitOfWork.WardRepository.GetDataAsync(expression);
        }
    }
}
