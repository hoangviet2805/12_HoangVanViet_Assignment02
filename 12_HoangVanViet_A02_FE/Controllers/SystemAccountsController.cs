using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Http;
using _12_HoangVanViet_A02_FE.Models;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly ApiService _apiService;

        public SystemAccountsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("Role") != 0)
            {
                return RedirectToAction("Login", "Account");
            }
            var accounts = await _apiService.GetSystemAccountsAsync();
            return View(accounts);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount(short id)
        {
            if (HttpContext.Session.GetInt32("Role") != 0) return Unauthorized();
            var account = await _apiService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();
            return Json(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SystemAccount account)
        {
            if (HttpContext.Session.GetInt32("Role") != 0) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            bool success = await _apiService.CreateAccountAsync(account);
            if (success) return Ok(new { message = "Account created successfully" });
            return StatusCode(500, "Error creating account");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SystemAccount account)
        {
            if (HttpContext.Session.GetInt32("Role") != 0) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            bool success = await _apiService.UpdateAccountAsync(account);
            if (success) return Ok(new { message = "Account updated successfully" });
            return StatusCode(500, "Error updating account");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short id)
        {
            if (HttpContext.Session.GetInt32("Role") != 0) return Unauthorized();
            
            string? error = await _apiService.DeleteAccountAsync(id);
            if (error == null) return Ok(new { message = "Account deleted successfully" });
            
            return BadRequest(new { message = error });
        }
    }
}
