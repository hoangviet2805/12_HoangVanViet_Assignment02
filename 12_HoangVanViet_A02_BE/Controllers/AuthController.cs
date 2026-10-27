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

        // Constructor: Sử dụng Dependency Injection để tiêm Repository và Configuration (đọc file appsettings.json)
        public AuthController(ISystemAccountRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        /// <summary>
        /// API Đăng nhập hệ thống (POST: /api/Auth/login)
        /// - Nhận Email và Password từ Client gửi lên.
        /// - Bước 1: Kiểm tra xem có phải tài khoản Admin cấu hình trong appsettings.json không.
        /// - Bước 2: Nếu không phải Admin, kiểm tra tài khoản trong Database qua Repository.
        /// - Bước 3: Nếu thông tin chính xác, sinh JWT Bearer Token chứa thông tin User và Role để trả về.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Kiểm tra dữ liệu đầu vào không được để trống
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Email and Password are required." });
            }

            // Đọc tài khoản Admin mặc định từ file appsettings.json theo yêu cầu đề bài
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            // TH1: Kiểm tra tài khoản Admin từ appsettings.json
            if (request.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && request.Password == adminPassword)
            {
                var adminAccount = new SystemAccount
                {
                    AccountID = 0,
                    AccountName = "Administrator",
                    AccountEmail = adminEmail,
                    AccountRole = 0 // Role 0 quy ước là Admin
                };

                // Sinh JWT Token với vai trò (Role) là Admin
                var token = GenerateJwtToken(adminAccount, "Admin");
                return Ok(new
                {
                    Token = token,
                    TokenType = "Bearer",
                    Account = adminAccount
                });
            }

            // TH2: Nếu không phải Admin, tìm tài khoản trong Database theo Email
            var account = _repository.GetAccountByEmail(request.Email);
            if (account != null && account.AccountPassword == request.Password)
            {
                // Ánh xạ mã số quyền sang tên Role chuẩn cho JWT: 1 = Staff, 2 = Lecturer
                string roleName = account.AccountRole switch
                {
                    1 => "Staff",
                    2 => "Lecturer",
                    _ => "User"
                };

                // Sinh JWT Token chứa Role tương ứng
                var token = GenerateJwtToken(account, roleName);
                return Ok(new
                {
                    Token = token,
                    TokenType = "Bearer",
                    Account = account
                });
            }

            // Nếu sai tài khoản hoặc mật khẩu thì trả về HTTP 401 Unauthorized
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        /// <summary>
        /// Hàm nội bộ: Sinh chuỗi JWT Token (Json Web Token)
        /// - Lấy cấu hình Secret Key, Issuer, Audience, thời hạn hết hạn từ appsettings.json.
        /// - Tạo Claims (User ID, Tên, Email, Role) để phân quyền ở các Controller khác bằng [Authorize].
        /// - Ký số Token bằng thuật toán HmacSha256 và khóa bảo mật.
        /// </summary>
        private string GenerateJwtToken(SystemAccount account, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? "FUNewsManagementSystem_Secret_Key_For_PRN232_Assignment02_HoangVanViet_12_2026";
            var issuer = jwtSettings["Issuer"] ?? "FUNewsManagementServer";
            var audience = jwtSettings["Audience"] ?? "FUNewsManagementClient";
            var durationMinutes = int.TryParse(jwtSettings["DurationInMinutes"], out var minutes) ? minutes : 120;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims: Chứa các thông tin định danh của người dùng đính kèm trong Payload của Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountID.ToString()),
                new Claim(ClaimTypes.Name, account.AccountName ?? "User"),
                new Claim(ClaimTypes.Email, account.AccountEmail ?? ""),
                new Claim(ClaimTypes.Role, roleName) // Dùng để phân quyền bằng [Authorize(Roles = "...")]
            };

            // Khởi tạo đối tượng JwtSecurityToken
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationMinutes), // Thời hạn sống của Token
                signingCredentials: credentials);

            // Ghi ra chuỗi Token dạng Base64
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO hứng dữ liệu gửi lên từ Form đăng nhập
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
