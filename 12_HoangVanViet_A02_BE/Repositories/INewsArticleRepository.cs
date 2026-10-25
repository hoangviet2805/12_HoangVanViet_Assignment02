using System.Collections.Generic;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public interface INewsArticleRepository
    {
        IEnumerable<NewsArticle> GetNewsArticles();
        NewsArticle? GetNewsArticleById(string id);
        void AddNewsArticle(NewsArticle article);
        void UpdateNewsArticle(NewsArticle article);
        void DeleteNewsArticle(NewsArticle article);
        IEnumerable<NewsArticle> GetStatistics(System.DateTime startDate, System.DateTime endDate);
    }
}
