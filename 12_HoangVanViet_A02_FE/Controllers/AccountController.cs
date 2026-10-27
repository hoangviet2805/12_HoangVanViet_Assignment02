using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    /// <summary>
    /// Controller quản lý việc Đăng nhập, Đăng xuất và lưu trữ Phiên người dùng (Session) trên Frontend
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        // Constructor Injection: Tiêm ApiService để gọi API Backend
        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Hiển thị màn hình Đăng nhập (GET: /Account/Login)
        /// - Nếu trong Session đã có token (người dùng đã đăng nhập trước đó) thì điều hướng thẳng về Trang chủ.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// Xử lý Submit Form Đăng nhập (POST: /Account/Login)
        /// - Nhận email và password từ Form view.
        /// - Gọi ApiService.LoginAsync() để xác thực với Backend API.
        /// - Nếu thành công:
        ///     1. Lưu chuỗi JWT Token vào Session["JWToken"].
        ///     2. Lưu Role và tên tài khoản vào Session để phân quyền hiển thị Menu.
        ///     3. Điều hướng người dùng về đúng trang theo vai trò:
        ///        + Admin -> Quản lý tài khoản (/SystemAccounts)
        ///        + Staff -> Quản lý bài viết (/NewsArticles)
        ///        + Lecturer -> Trang hồ sơ cá nhân (/Profile)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both Email and Password.";
                return View();
            }

            var result = await _apiService.LoginAsync(email, password);
            if (result != null && result.Account != null && !string.IsNullOrEmpty(result.Token))
            {
                var account = result.Account;
                var role = account.AccountRole ?? 2;
                string roleName = role switch
                {
                    0 => "Admin",
                    1 => "Staff",
                    2 => "Lecturer",
                    _ => "User"
                };

                // Lưu các thông tin bảo mật vào Session của ứng dụng MVC
                HttpContext.Session.SetString("JWToken", result.Token);
                HttpContext.Session.SetString("User", JsonSerializer.Serialize(account));
                HttpContext.Session.SetInt32("Role", role);
                HttpContext.Session.SetString("RoleName", roleName);
                HttpContext.Session.SetInt32("AccountId", account.AccountID);
                HttpContext.Session.SetString("AccountName", account.AccountName ?? "User");

                // Điều hướng theo đúng vai trò người dùng
                if (roleName == "Admin")
                {
                    return RedirectToAction("Index", "SystemAccounts");
                }
                else if (roleName == "Staff")
                {
                    return RedirectToAction("Index", "NewsArticles");
                }
                else if (roleName == "Lecturer")
                {
                    return RedirectToAction("Index", "Profile");
                }
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        /// <summary>
        /// Xử lý Đăng xuất (GET: /Account/Logout)
        /// - Xóa sạch toàn bộ dữ liệu Session hiện tại (Token, Role, User).
        /// - Điều hướng người dùng quay lại màn hình Login.
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
