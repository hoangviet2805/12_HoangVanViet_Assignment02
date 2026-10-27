using _12_HoangVanViet_A02_BE.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _12_HoangVanViet_A02_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Toàn bộ Controller này chỉ dành riêng cho Admin (Báo cáo thống kê)
    public class ReportsController : ControllerBase
    {
        private readonly INewsArticleRepository _repository;

        // Constructor Injection: Tiêm INewsArticleRepository để truy vấn dữ liệu bài viết
        public ReportsController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// API Thống kê bài viết theo khoảng ngày (GET: /api/Reports/statistics?startDate=...&endDate=...)
        /// - Nhận 2 tham số: startDate (Ngày bắt đầu) và endDate (Ngày kết thúc) từ Query string.
        /// - Kiểm tra tính hợp lệ: startDate không được lớn hơn endDate.
        /// - Gọi Repository để lọc bài viết có CreatedDate nằm trong khoảng đó và sắp xếp giảm dần theo ngày tạo.
        /// - Trả về danh sách bài viết phục vụ vẽ biểu đồ/bảng báo cáo cho Admin.
        /// </summary>
        [HttpGet("statistics")]
        public IActionResult GetStatistics([FromQuery] System.DateTime startDate, [FromQuery] System.DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { Message = "StartDate cannot be greater than EndDate." });
            }
            var statistics = _repository.GetStatistics(startDate, endDate);
            return Ok(statistics);
        }
    }
}
