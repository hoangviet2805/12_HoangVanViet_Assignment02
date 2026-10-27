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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        // Constructor Injection: Tiêm ICategoryRepository
        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// API Lấy toàn bộ danh mục (GET: /api/Categories)
        /// - [AllowAnonymous]: Cho phép gọi công khai để hiển thị trên Dropdown chọn danh mục khi xem tin tức hoặc lọc.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetCategories());
        }

        /// <summary>
        /// API Tạo mới danh mục (POST: /api/Categories)
        /// - Phân quyền: Chỉ Staff mới được tạo danh mục ([Authorize(Roles = "Staff")]).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public IActionResult Post([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.AddCategory(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryID }, category);
        }

        /// <summary>
        /// API Truy vấn OData danh mục (GET: /odata/Categories)
        /// - Cho phép lọc theo trạng thái IsActive, sắp xếp hoặc tìm theo tên qua $filter, $orderby.
        /// </summary>
        [HttpGet("/odata/Categories")]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_repository.GetCategories().AsQueryable());
        }

        /// <summary>
        /// API Đếm số lượng danh mục (GET: /odata/Categories/$count)
        /// </summary>
        [HttpGet("/odata/Categories/$count")]
        [AllowAnonymous]
        public IActionResult GetODataCount()
        {
            return Ok(_repository.GetCategories().Count());
        }

        /// <summary>
        /// API Lấy chi tiết danh mục theo ID (GET: /api/Categories/{id})
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetCategory(short id)
        {
            var category = _repository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        /// <summary>
        /// API Cập nhật danh mục (PUT: /api/Categories/{id})
        /// - Phân quyền: Chỉ Staff mới được sửa ([Authorize(Roles = "Staff")]).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Put(short id, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != category.CategoryID)
            {
                return BadRequest();
            }

            var existing = _repository.GetCategoryById(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.CategoryName = category.CategoryName;
            existing.CategoryDesciption = category.CategoryDesciption;
            existing.ParentCategoryID = category.ParentCategoryID;
            existing.IsActive = category.IsActive;

            _repository.UpdateCategory(existing);
            return Ok(existing);
        }

        /// <summary>
        /// API Xóa danh mục (DELETE: /api/Categories/{id})
        /// - Phân quyền: Chỉ Staff mới có quyền xóa ([Authorize(Roles = "Staff")]).
        /// - Nghiệp vụ quan trọng: Nếu danh mục đã có bài viết tham chiếu thì tầng DAO sẽ ném lỗi InvalidOperationException,
        ///   hàm này sẽ bắt lỗi và trả về HTTP 400 kèm thông báo ngăn không cho xóa.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public IActionResult Delete(short id)
        {
            var category = _repository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            try
            {
                _repository.DeleteCategory(category);
                return NoContent();
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
