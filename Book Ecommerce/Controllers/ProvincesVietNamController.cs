using Book_Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Ecommerce.Controllers
{
    public class ProvincesVietNamController : Controller
    {
        private readonly AppDbContext _context;

        public ProvincesVietNamController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProvince()
        {
            var provinces = await _context.Provinces.ToListAsync();
            return Json(new { data = provinces});
        }
        [HttpGet]
        public async Task<IActionResult> GetDistrictsByProvince(string provinceCode)
        {
            var districts = await _context.Districts.Where(d => d.ProvinceCode == provinceCode).ToListAsync();
            return Json(new { data = districts });
        }
        [HttpGet]
        public async Task<IActionResult> GetWardsByDistrict(string districtCode)
        {
            var wards = await _context.Wards.Where(w => w.DistrictCode == districtCode).ToListAsync();
            return Json(new { data = wards });
        }
        [HttpGet]
        public async Task<IActionResult> GetProvinceById(string code)
        {
            var province = await _context.Provinces.FirstOrDefaultAsync(p => p.Code == code);
            return Json(new { data = province });
        }
        [HttpGet]
        public async Task<IActionResult> GetDistrictsById(string code)
        {
            var district = await _context.Districts.FirstOrDefaultAsync(p => p.Code == code); ;
            return Json(new { data = district });
        }
        [HttpGet]
        public async Task<IActionResult> GetWardsById(string code)
        {
            var ward = await _context.Wards.FirstOrDefaultAsync(p => p.Code == code);
            return Json(new { data = ward });
        }
    }
}
