using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Http;
using _12_HoangVanViet_A02_FE.Models;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiService _apiService;

        public ProfileController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            var id = HttpContext.Session.GetInt32("AccountId");
            if (role != 1 && role != 2 || id == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var account = await _apiService.GetAccountByIdAsync((short)id.Value);
            if (account == null) return NotFound();
            
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SystemAccount account)
        {
            var id = HttpContext.Session.GetInt32("AccountId");
            if (id == null || id.Value != account.AccountID) return Unauthorized();
            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            bool success = await _apiService.UpdateProfileAsync(account);
            if (success)
            {
                HttpContext.Session.SetString("AccountName", account.AccountName ?? "User");
                return Ok(new { message = "Profile updated successfully" });
            }
            return StatusCode(500, "Error updating profile");
        }
    }
}
