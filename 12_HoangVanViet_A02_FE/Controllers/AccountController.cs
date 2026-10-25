using _12_HoangVanViet_A02_FE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace _12_HoangVanViet_A02_FE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to home
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

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

                HttpContext.Session.SetString("JWToken", result.Token);
                HttpContext.Session.SetString("User", JsonSerializer.Serialize(account));
                HttpContext.Session.SetInt32("Role", role);
                HttpContext.Session.SetString("RoleName", roleName);
                HttpContext.Session.SetInt32("AccountId", account.AccountID);
                HttpContext.Session.SetString("AccountName", account.AccountName ?? "User");

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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
