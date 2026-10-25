using System.Collections.Generic;
using _12_HoangVanViet_A02_BE.DAOs;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public void AddNewsArticle(NewsArticle article) => NewsArticleDAO.Instance.AddNewsArticle(article);
        public void DeleteNewsArticle(NewsArticle article) => NewsArticleDAO.Instance.DeleteNewsArticle(article);
        public NewsArticle? GetNewsArticleById(string id) => NewsArticleDAO.Instance.GetNewsArticleById(id);
        public IEnumerable<NewsArticle> GetNewsArticles() => NewsArticleDAO.Instance.GetNewsArticles();
        public void UpdateNewsArticle(NewsArticle article) => NewsArticleDAO.Instance.UpdateNewsArticle(article);
        public IEnumerable<NewsArticle> GetStatistics(System.DateTime startDate, System.DateTime endDate) => NewsArticleDAO.Instance.GetStatistics(startDate, endDate);
    }
}
