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
    public class AccountsController : ControllerBase
    {
        private readonly ISystemAccountRepository _repository;

        public AccountsController(ISystemAccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAccounts());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.AddAccount(account);
            return CreatedAtAction(nameof(GetAccount), new { id = account.AccountID }, account);
        }

        [HttpGet("/odata/Accounts")]
        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_repository.GetAccounts().AsQueryable());
        }

        [HttpGet("/odata/Accounts/$count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetODataCount()
        {
            return Ok(_repository.GetAccounts().Count());
        }

        [HttpGet("{id}")]
        [Authorize] // Admin, Staff, Lecturer can get account by id
        public IActionResult GetAccount(short id)
        {
            var account = _repository.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(short id, [FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != account.AccountID)
            {
                return BadRequest();
            }

            var existing = _repository.GetAccountById(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.AccountName = account.AccountName;
            existing.AccountEmail = account.AccountEmail;
            existing.AccountRole = account.AccountRole;
            existing.AccountPassword = account.AccountPassword;

            _repository.UpdateAccount(existing);
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(short id)
        {
            var account = _repository.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }
            try
            {
                _repository.DeleteAccount(account);
                return NoContent();
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("profile/{id}")]
        [Authorize] // Any authenticated user (Admin or Staff) can update their profile
        public IActionResult UpdateProfile(short id, [FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != account.AccountID)
            {
                return BadRequest();
            }

            var existing = _repository.GetAccountById(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.AccountName = account.AccountName;
            existing.AccountEmail = account.AccountEmail;
            existing.AccountPassword = account.AccountPassword;

            _repository.UpdateAccount(existing);
            return Ok(existing);
        }
    }
}
