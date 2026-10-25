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

        public NewsArticlesController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetNewsArticles());
        }

        [HttpGet("/odata/NewsArticles")]
        [AllowAnonymous] // Allow public querying or authenticated querying
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_repository.GetNewsArticles().AsQueryable());
        }

        [HttpGet("/odata/NewsArticles/$count")]
        [AllowAnonymous]
        public IActionResult GetODataCount()
        {
            return Ok(_repository.GetNewsArticles().Count());
        }

        /// <summary>
        /// Public endpoint: No authentication needed to view active news articles.
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public IActionResult GetActive()
        {
            var activeArticles = _repository.GetNewsArticles().Where(a => a.NewsStatus == true).ToList();
            return Ok(activeArticles);
        }

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

        [HttpGet("{id}")]
        [AllowAnonymous] // Public can view article details if active; staff can view any
        public IActionResult GetNewsArticle(string id)
        {
            var article = _repository.GetNewsArticleById(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

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

        [HttpGet("author/{authorId}")]
        [Authorize(Roles = "Staff")]
        public IActionResult GetByAuthor(short authorId)
        {
            var articles = _repository.GetNewsArticles().Where(a => a.CreatedByID == authorId).ToList();
            return Ok(articles);
        }
    }
}
