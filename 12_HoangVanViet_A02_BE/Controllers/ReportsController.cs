using _12_HoangVanViet_A02_BE.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _12_HoangVanViet_A02_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly INewsArticleRepository _repository;

        public ReportsController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

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
