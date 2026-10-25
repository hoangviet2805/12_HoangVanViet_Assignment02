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

        public async Task<List<NewsArticle>> GetActiveNewsArticlesAsync()
        {
            // Public endpoint: does not require token
            var response = await _httpClient.GetAsync($"{_baseUrl}api/NewsArticles/active");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<NewsArticle>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<NewsArticle>();
            }
            return new List<NewsArticle>();
        }

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

        public async Task<bool> CreateNewsArticleAsync(NewsArticle article)
        {
            AttachAuthToken();
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}api/NewsArticles", article);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNewsArticleAsync(NewsArticle article)
        {
            AttachAuthToken();
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}api/NewsArticles/{article.NewsArticleID}", article);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            AttachAuthToken();
            var response = await _httpClient.DeleteAsync($"{_baseUrl}api/NewsArticles/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Reports ---
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
