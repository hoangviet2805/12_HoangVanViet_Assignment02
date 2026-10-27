using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using _12_HoangVanViet_A02_FE.Models;
using System.Text.Json;

namespace _12_HoangVanViet_A02_FE.Services
{
    /// <summary>
    /// Service trung tâm quản lý toàn bộ việc gọi HTTP API sang Backend
    /// - Sử dụng HttpClient tiêm qua Dependency Injection.
    /// - Tự động đính kèm JWT Token vào Header của mỗi Request.
    /// - Xử lý bóc tách định dạng dữ liệu trả về từ Web API và OData.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7279/";
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Hàm nội bộ: Tự động trích xuất chuỗi JWT Token từ HttpContext.Session của người dùng
        /// và gắn vào Request Header: "Authorization: Bearer <token>" trước khi gửi sang Backend.
        /// </summary>
        private void AttachAuthToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// Gọi API Đăng nhập sang Backend (POST: /api/Auth/login)
        /// - Nhận về Token và thông tin User nếu thành công.
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/Auth/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<LoginResponse>(options);
            }
            return null;
        }

        // --- Categories ---

        /// <summary>
        /// Lấy danh sách danh mục qua OData (/odata/Categories)
        /// - Có cơ chế thông minh: Kiểm tra nếu OData trả về bọc trong thuộc tính {"value": [...]} thì bóc tách lấy mảng bên trong.
        /// </summary>
        public async Task<List<Category>> GetCategoriesAsync()
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}odata/Categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return jsonDoc.RootElement.Deserialize<List<Category>>(options) ?? new List<Category>();
                }
                else if (jsonDoc.RootElement.TryGetProperty("value", out var valueProp))
                {
                    return valueProp.Deserialize<List<Category>>(options) ?? new List<Category>();
                }
            }
            return new List<Category>();
        }

        public async Task<Category?> GetCategoryByIdAsync(short id)
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Category>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            AttachAuthToken();
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/Categories", category);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            AttachAuthToken();
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/Categories/{category.CategoryID}", category);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> DeleteCategoryAsync(short id)
        {
            AttachAuthToken();
            var response = await _httpClient.DeleteAsync($"{_baseUrl}api/Categories/{id}");
            if (response.IsSuccessStatusCode) return null; // Success

            var error = await response.Content.ReadAsStringAsync();
            try
            {
                var jsonDoc = JsonDocument.Parse(error);
                if (jsonDoc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    return msgProp.GetString();
                }
            }
            catch
            {
                // Return plain string if not json
            }
            return string.IsNullOrEmpty(error) ? "Failed to delete category" : error;
        }

        // --- System Accounts ---
        public async Task<List<SystemAccount>> GetSystemAccountsAsync()
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}odata/Accounts");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return jsonDoc.RootElement.Deserialize<List<SystemAccount>>(options) ?? new List<SystemAccount>();
                }
                else if (jsonDoc.RootElement.TryGetProperty("value", out var valueProp))
                {
                    return valueProp.Deserialize<List<SystemAccount>>(options) ?? new List<SystemAccount>();
                }
            }
            return new List<SystemAccount>();
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short id)
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Accounts/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SystemAccount>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<bool> CreateAccountAsync(SystemAccount account)
        {
            AttachAuthToken();
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/Accounts", account);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAccountAsync(SystemAccount account)
        {
            AttachAuthToken();
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/Accounts/{account.AccountID}", account);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProfileAsync(SystemAccount account)
        {
            AttachAuthToken();
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/Accounts/profile/{account.AccountID}", account);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> DeleteAccountAsync(short id)
        {
            AttachAuthToken();
            var response = await _httpClient.DeleteAsync($"{_baseUrl}api/Accounts/{id}");
            if (response.IsSuccessStatusCode) return null; // Success

            var error = await response.Content.ReadAsStringAsync();
            try
            {
                var jsonDoc = JsonDocument.Parse(error);
                if (jsonDoc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    return msgProp.GetString();
                }
            }
            catch
            {
                // Return plain string if not json
            }
            return string.IsNullOrEmpty(error) ? "Failed to delete account" : error;
        }

        // --- News Articles ---

        /// <summary>
        /// Lấy toàn bộ bài viết (Dành cho Staff/Admin)
        /// </summary>
        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/NewsArticles");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NewsArticle>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<NewsArticle>();
            }
            return new List<NewsArticle>();
        }

        /// <summary>
        /// Lấy các bài viết đang kích hoạt (Active) dành cho khách xem trang chủ (Không cần gắn Token)
        /// </summary>
        public async Task<List<NewsArticle>> GetActiveNewsArticlesAsync()
        {
            // Endpoint công khai: không cần đính kèm Bearer token
            var response = await _httpClient.GetAsync($"{_baseUrl}api/NewsArticles/active");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NewsArticle>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<NewsArticle>();
            }
            return new List<NewsArticle>();
        }

        /// <summary>
        /// Lấy lịch sử các bài viết do 1 tác giả cụ thể tạo (Dành cho trang quản lý của Staff)
        /// </summary>
        public async Task<List<NewsArticle>> GetNewsHistoryAsync(short authorId)
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/NewsArticles/author/{authorId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NewsArticle>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<NewsArticle>();
            }
            return new List<NewsArticle>();
        }

        /// <summary>
        /// Lấy chi tiết bài viết theo ID
        /// </summary>
        public async Task<NewsArticle?> GetNewsArticleByIdAsync(string id)
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/NewsArticles/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<NewsArticle>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        /// <summary>
        /// Tạo mới bài viết: Gắn Token và bắn HTTP POST sang Backend
        /// </summary>
        public async Task<bool> CreateNewsArticleAsync(NewsArticle article)
        {
            AttachAuthToken();
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/NewsArticles", article);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Cập nhật bài viết: Gắn Token và bắn HTTP PUT sang Backend
        /// </summary>
        public async Task<bool> UpdateNewsArticleAsync(NewsArticle article)
        {
            AttachAuthToken();
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/NewsArticles/{article.NewsArticleID}", article);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Xóa bài viết: Gắn Token và bắn HTTP DELETE sang Backend
        /// </summary>
        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            AttachAuthToken();
            var response = await _httpClient.DeleteAsync($"{_baseUrl}api/NewsArticles/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Reports ---

        /// <summary>
        /// Lấy danh sách thống kê bài viết theo khoảng ngày (Dành cho Admin)
        /// </summary>
        public async Task<List<NewsArticle>> GetStatisticsAsync(System.DateTime startDate, System.DateTime endDate)
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Reports/statistics?startDate={startDate:O}&endDate={endDate:O}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NewsArticle>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<NewsArticle>();
            }
            return new List<NewsArticle>();
        }

        // --- Tags ---

        /// <summary>
        /// Lấy danh sách toàn bộ các Tag bài viết
        /// </summary>
        public async Task<List<Tag>> GetTagsAsync()
        {
            AttachAuthToken();
            var response = await _httpClient.GetAsync($"{_baseUrl}api/Tags");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Tag>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Tag>();
            }
            return new List<Tag>();
        }
    }
}
