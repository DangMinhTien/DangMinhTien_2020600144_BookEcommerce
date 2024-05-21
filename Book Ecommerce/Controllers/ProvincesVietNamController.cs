using Book_Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book_Ecommerce.Data;
using Book_Ecommerce.Service.Abstract;
using Book_Ecommerce.Service;

namespace Book_Ecommerce.Controllers
{
    public class ProvincesVietNamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProvincesVietNamController> _logger;
        private readonly IProvinceService _provinceService;

        public ProvincesVietNamController(AppDbContext context,
            ILogger<ProvincesVietNamController> logger,
            IProvinceService provinceService)
        {
            _context = context;
            _logger = logger;
            _provinceService = provinceService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/lay-tinh-thanh")]
        public async Task<IActionResult> GetAllProvince()
        {
            try
            {
                var provinces = await _provinceService.GetDataProvinceAsync();
                return Json(new { data = provinces});
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet("/lay-quan-huyen-theo-tinh-thanh")]
        public async Task<IActionResult> GetDistrictsByProvince(string provinceCode)
        {
            try
            {
                var districts = await _provinceService.GetDataDistrictAsync(d => d.ProvinceCode == provinceCode);
                return Json(new { data = districts });

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet("/lay-xa-phuong-theo-quan-huyen")]
        public async Task<IActionResult> GetWardsByDistrict(string districtCode)
        {
            try
            {
                var wards = await _provinceService.GetDataWardAsync(w => w.DistrictCode == districtCode);
                return Json(new { data = wards });

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet("/lay-tinh-thanh-theo-id")]
        public async Task<IActionResult> GetProvinceById(string code)
        {
            try
            {
                var province = await _provinceService.GetSingleProvinceByConditionAsync(p => p.Code == code);
                return Json(new { data = province });

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet("/lay-quan-huyen-theo-id")]
        public async Task<IActionResult> GetDistrictsById(string code)
        {
            try
            {
                var district = await _provinceService.GetSingleDistrictByConditionAsync(p => p.Code == code); ;
                return Json(new { data = district });

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet("/lay-quan-huyen-theo-id")]
        public async Task<IActionResult> GetWardsById(string code)
        {
            try
            {
                var ward = await _provinceService.GetSingleWardByConditionAsync(p => p.Code == code);
                return Json(new { data = ward });
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
    }
}
