using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Http;
using System;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApiService _apiService;

        public ReportController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("Role") != 0) // Admin only
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics(DateTime startDate, DateTime endDate)
        {
            if (HttpContext.Session.GetInt32("Role") != 0) return Unauthorized();
            
            var statistics = await _apiService.GetStatisticsAsync(startDate, endDate);
            return Json(statistics);
        }
    }
}
