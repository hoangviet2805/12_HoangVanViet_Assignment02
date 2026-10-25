using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Http;
using _12_HoangVanViet_A02_FE.Models;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;

        public CategoriesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) // Staff or Lecturer
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = await _apiService.GetCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(short id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Json(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            bool success = await _apiService.CreateCategoryAsync(category);
            if (success) return Ok(new { message = "Category created successfully" });
            return StatusCode(500, "Error creating category");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Category category)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            bool success = await _apiService.UpdateCategoryAsync(category);
            if (success) return Ok(new { message = "Category updated successfully" });
            return StatusCode(500, "Error updating category");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            
            string? error = await _apiService.DeleteCategoryAsync(id);
            if (error == null) return Ok(new { message = "Category deleted successfully" });
            
            return BadRequest(new { message = error });
        }
    }
}
