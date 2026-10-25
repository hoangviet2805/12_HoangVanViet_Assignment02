using _12_HoangVanViet_A02_BE.Models;
using _12_HoangVanViet_A02_BE.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _12_HoangVanViet_A02_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISystemAccountRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthController(ISystemAccountRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Email and Password are required." });
            }

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            // Check admin from configuration
            if (request.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && request.Password == adminPassword)
            {
                var adminAccount = new SystemAccount
                {
                    AccountID = 0,
                    AccountName = "Administrator",
                    AccountEmail = adminEmail,
                    AccountRole = 0 // Admin
                };

                var token = GenerateJwtToken(adminAccount, "Admin");
                return Ok(new
                {
                    Token = token,
                    TokenType = "Bearer",
                    Account = adminAccount
                });
            }

            // Check database accounts
            var account = _repository.GetAccountByEmail(request.Email);
            if (account != null && account.AccountPassword == request.Password)
            {
                string roleName = account.AccountRole switch
                {
                    1 => "Staff",
                    2 => "Lecturer",
                    _ => "User"
                };

                var token = GenerateJwtToken(account, roleName);
                return Ok(new
                {
                    Token = token,
                    TokenType = "Bearer",
                    Account = account
                });
            }

            return Unauthorized(new { Message = "Invalid email or password." });
        }

        private string GenerateJwtToken(SystemAccount account, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? "FUNewsManagementSystem_Secret_Key_For_PRN232_Assignment02_HoangVanViet_12_2026";
            var issuer = jwtSettings["Issuer"] ?? "FUNewsManagementServer";
            var audience = jwtSettings["Audience"] ?? "FUNewsManagementClient";
            var durationMinutes = int.TryParse(jwtSettings["DurationInMinutes"], out var minutes) ? minutes : 120;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountID.ToString()),
                new Claim(ClaimTypes.Name, account.AccountName ?? "User"),
                new Claim(ClaimTypes.Email, account.AccountEmail ?? ""),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
