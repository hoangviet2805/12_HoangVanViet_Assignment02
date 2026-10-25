using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Http;
using _12_HoangVanViet_A02_FE.Models;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly ApiService _apiService;

        public NewsArticlesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetInt32("Role");
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (role != 1 && role != 2 || accountId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var articles = await _apiService.GetNewsHistoryAsync((short)accountId.Value);
            ViewBag.Categories = await _apiService.GetCategoriesAsync();
            ViewBag.Tags = await _apiService.GetTagsAsync();
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> GetArticle(string id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            
            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null) return NotFound();
            return Json(article);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewsArticle article)
        {
            var role = HttpContext.Session.GetInt32("Role");
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (role != 1 && role != 2 || accountId == null) return Unauthorized();
            
            if (string.IsNullOrEmpty(article.NewsArticleID))
            {
                article.NewsArticleID = System.Guid.NewGuid().ToString("N").Substring(0, 15);
                ModelState.Remove("NewsArticleID");
            }
            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            article.CreatedByID = (short)accountId.Value;
            article.UpdatedByID = (short)accountId.Value;
            article.CreatedDate = System.DateTime.UtcNow;
            
            bool success = await _apiService.CreateNewsArticleAsync(article);
            if (success) return Ok(new { message = "Article created successfully" });
            return StatusCode(500, "Error creating article");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NewsArticle article)
        {
            var role = HttpContext.Session.GetInt32("Role");
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (role != 1 && role != 2 || accountId == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            article.UpdatedByID = (short)accountId.Value;
            
            bool success = await _apiService.UpdateNewsArticleAsync(article);
            if (success) return Ok(new { message = "Article updated successfully" });
            return StatusCode(500, "Error updating article");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            
            bool success = await _apiService.DeleteNewsArticleAsync(id);
            if (success) return Ok(new { message = "Article deleted successfully" });
            
            return BadRequest(new { message = "Failed to delete article" });
        }
    }
}
