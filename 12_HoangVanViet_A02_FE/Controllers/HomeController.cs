using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _12_HoangVanViet_A02_FE.Models;
using _12_HoangVanViet_A02_FE.Services;
using System.Threading.Tasks;

namespace _12_HoangVanViet_A02_FE.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiService _apiService;

    public HomeController(ILogger<HomeController> logger, ApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string searchString, short? categoryId)
    {
        var activeNews = await _apiService.GetActiveNewsArticlesAsync();
        
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            activeNews = activeNews.Where(n => 
                (n.NewsTitle != null && n.NewsTitle.ToLower().Contains(searchString)) ||
                (n.Headline != null && n.Headline.ToLower().Contains(searchString)) ||
                (n.NewsContent != null && n.NewsContent.ToLower().Contains(searchString))
            ).ToList();
        }

        if (categoryId.HasValue)
        {
            activeNews = activeNews.Where(n => n.CategoryID == categoryId.Value).ToList();
        }

        ViewBag.Categories = await _apiService.GetCategoriesAsync();
        ViewBag.SearchString = searchString;
        ViewBag.CategoryId = categoryId;
        
        return View(activeNews);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var article = await _apiService.GetNewsArticleByIdAsync(id);
        if (article == null || article.NewsStatus != true) return NotFound();
        return View(article);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
