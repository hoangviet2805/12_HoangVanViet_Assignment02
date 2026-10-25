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
    [Authorize(Roles = "Staff,Admin")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _repository;

        public TagsController(ITagRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetTags());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.AddTag(tag);
            return CreatedAtAction(nameof(GetTag), new { id = tag.TagID }, tag);
        }

        [HttpGet("/odata/Tags")]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_repository.GetTags().AsQueryable());
        }

        [HttpGet("/odata/Tags/$count")]
        public IActionResult GetODataCount()
        {
            return Ok(_repository.GetTags().Count());
        }

        [HttpGet("{id}")]
        public IActionResult GetTag(int id)
        {
            var tag = _repository.GetTagById(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != tag.TagID)
            {
                return BadRequest();
            }

            var existing = _repository.GetTagById(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.TagName = tag.TagName;
            existing.Note = tag.Note;

            _repository.UpdateTag(existing);
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var tag = _repository.GetTagById(id);
            if (tag == null)
            {
                return NotFound();
            }
            _repository.DeleteTag(tag);
            return NoContent();
        }
    }
}
