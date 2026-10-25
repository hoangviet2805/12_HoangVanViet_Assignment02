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

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetCategories());
        }

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

        [HttpGet("/odata/Categories")]
        [AllowAnonymous]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_repository.GetCategories().AsQueryable());
        }

        [HttpGet("/odata/Categories/$count")]
        [AllowAnonymous]
        public IActionResult GetODataCount()
        {
            return Ok(_repository.GetCategories().Count());
        }

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
