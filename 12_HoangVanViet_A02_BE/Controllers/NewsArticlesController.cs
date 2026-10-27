using _12_HoangVanViet_A02_BE.Models;
using _12_HoangVanViet_A02_BE.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;

namespace _12_HoangVanViet_A02_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleRepository _repository;

        // Constructor: Nhận INewsArticleRepository thông qua cơ chế Dependency Injection (DI)
        public NewsArticlesController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// API Lấy toàn bộ danh sách bài viết (GET: /api/NewsArticles)
        /// - Phân quyền: Chỉ Staff hoặc Admin mới có quyền truy cập ([Authorize(Roles = "Staff,Admin")]).
        /// - Gọi Repository lấy toàn bộ bài viết (bao gồm cả active và inactive).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetNewsArticles());
        }

        /// <summary>
        /// API Truy vấn OData (GET: /odata/NewsArticles)
        /// - [EnableQuery]: Cho phép Client truyền các tham số OData ($filter, $select, $orderby, $expand, $top, $skip).
        /// - [AllowAnonymous]: Cho phép gọi công khai mà không cần đăng nhập.
        /// - Trả về IQueryable để OData tự động tối ưu hóa câu truy vấn.
        /// </summary>
        [HttpGet("/odata/NewsArticles")]
        [AllowAnonymous] // Cho phép truy vấn công khai hoặc đã đăng nhập
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_repository.GetNewsArticles().AsQueryable());
        }

        /// <summary>
        /// API Đếm số lượng bài viết theo chuẩn OData (GET: /odata/NewsArticles/$count)
        /// </summary>
        [HttpGet("/odata/NewsArticles/$count")]
        [AllowAnonymous]
        public IActionResult GetODataCount()
        {
            return Ok(_repository.GetNewsArticles().Count());
        }

        /// <summary>
        /// API Lấy danh sách bài viết đang kích hoạt (GET: /api/NewsArticles/active)
        /// - Dành cho khách vãng lai ở trang chủ (Home): Không cần đăng nhập ([AllowAnonymous]).
        /// - Chỉ trả về các bài viết có NewsStatus == true.
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public IActionResult GetActive()
        {
            var activeArticles = _repository.GetNewsArticles().Where(a => a.NewsStatus == true).ToList();
            return Ok(activeArticles);
        }

        /// <summary>
        /// API Tạo mới bài viết (POST: /api/NewsArticles)
        /// - Phân quyền: Chỉ Staff mới có quyền đăng bài ([Authorize(Roles = "Staff")]).
        /// - Kiểm tra ModelState, sau đó gọi Repository thêm vào CSDL.
        /// - Trả về mã HTTP 201 Created kèm URL bài viết vừa tạo.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult Post([FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.AddNewsArticle(article);
            return CreatedAtAction(nameof(GetNewsArticle), new { id = article.NewsArticleID }, article);
        }

        /// <summary>
        /// API Lấy chi tiết bài viết theo ID (GET: /api/NewsArticles/{id})
        /// - Cho phép xem công khai ([AllowAnonymous]).
        /// - Nếu không tìm thấy thì trả về HTTP 404 NotFound.
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetNewsArticle(string id)
        {
            var article = _repository.GetNewsArticleById(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        /// <summary>
        /// API Cập nhật bài viết (PUT: /api/NewsArticles/{id})
        /// - Phân quyền: Chỉ Staff mới có quyền sửa bài ([Authorize(Roles = "Staff")]).
        /// - Tìm bài viết cũ trong DB, nếu không có thì trả về NotFound.
        /// - Gán dữ liệu mới (Tiêu đề, nội dung, danh mục, trạng thái, người sửa, ngày sửa ModifiedDate = UtcNow).
        /// - Gọi Repository để cập nhật vào Database.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Put(string id, [FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != article.NewsArticleID)
            {
                return BadRequest();
            }

            var existing = _repository.GetNewsArticleById(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.CategoryID = article.CategoryID;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedByID = article.UpdatedByID;
            existing.ModifiedDate = System.DateTime.UtcNow;

            _repository.UpdateNewsArticle(existing);
            return Ok(existing);
        }

        /// <summary>
        /// API Xóa bài viết theo ID (DELETE: /api/NewsArticles/{id})
        /// - Phân quyền: Chỉ Staff mới có quyền xóa bài ([Authorize(Roles = "Staff")]).
        /// - Tìm bài viết trong DB, nếu có thì gọi Repository để xóa, trả về HTTP 204 NoContent.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Delete(string id)
        {
            var article = _repository.GetNewsArticleById(id);
            if (article == null)
            {
                return NotFound();
            }
            _repository.DeleteNewsArticle(article);
            return NoContent();
        }

        /// <summary>
        /// API Lấy lịch sử các bài viết do 1 tác giả tạo (GET: /api/NewsArticles/author/{authorId})
        /// - Dành cho tính năng "Manage News Articles History" của Staff.
        /// - Lọc theo CreatedByID == authorId.
        /// </summary>
        [HttpGet("author/{authorId}")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetByAuthor(short authorId)
        {
            var articles = _repository.GetNewsArticles().Where(a => a.CreatedByID == authorId).ToList();
            return Ok(articles);
        }
    }
}
