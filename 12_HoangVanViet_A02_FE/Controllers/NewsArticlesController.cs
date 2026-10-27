using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Http;
using _12_HoangVanViet_A02_FE.Models;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    /// <summary>
    /// Controller Frontend quản lý Bài báo dành cho Staff và Lecturer
    /// - Nhận request từ View (Ajax/Form), kiểm tra quyền Session và gọi ApiService để thao tác với Backend API.
    /// </summary>
    public class NewsArticlesController : Controller
    {
        private readonly ApiService _apiService;

        // Constructor: Tiêm ApiService
        public NewsArticlesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Màn hình Lịch sử bài viết của tôi (GET: /NewsArticles/Index)
        /// - Kiểm tra Session: Chỉ cho phép Role 1 (Staff) hoặc Role 2 (Lecturer). Nếu chưa login thì chuyển về trang Login.
        /// - Lấy lịch sử bài viết của chính tác giả đó qua GetNewsHistoryAsync(accountId).
        /// - Nạp danh sách Categories và Tags vào ViewBag để hiển thị trên Dropdown của Modal tạo/sửa.
        /// </summary>
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

        /// <summary>
        /// Lấy thông tin 1 bài viết dạng JSON để đổ dữ liệu lên Modal Edit (GET: /NewsArticles/GetArticle?id=...)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetArticle(string id)
        {
            var role = HttpContext.Session.GetInt32("Role");
            if (role != 1 && role != 2) return Unauthorized();
            
            var article = await _apiService.GetNewsArticleByIdAsync(id);
            if (article == null) return NotFound();
            return Json(article);
        }

        /// <summary>
        /// Xử lý tạo mới bài viết từ Modal Ajax (POST: /NewsArticles/Create)
        /// - Kiểm tra quyền trong Session.
        /// - Tự động sinh ID bài viết ngẫu nhiên 15 ký tự nếu chưa có.
        /// - Tự động gán CreatedByID và UpdatedByID theo ID người đăng nhập trong Session.
        /// - Gán ngày tạo CreatedDate = DateTime.UtcNow.
        /// - Gọi ApiService.CreateNewsArticleAsync(article) gửi sang Backend.
        /// </summary>
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

        /// <summary>
        /// Xử lý cập nhật bài viết từ Modal Ajax (PUT: /NewsArticles/Update)
        /// - Tự động gán UpdatedByID theo ID người sửa trong Session.
        /// - Gọi ApiService.UpdateNewsArticleAsync(article) gửi sang Backend.
        /// </summary>
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

        /// <summary>
        /// Xử lý xóa bài viết qua Ajax (DELETE: /NewsArticles/Delete/{id})
        /// - Gọi ApiService.DeleteNewsArticleAsync(id) gửi sang Backend.
        /// </summary>
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
