using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.DAOs
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance;
        private static readonly object _instanceLock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NewsArticleDAO();
                    }
                    return _instance;
                }
            }
        }

        public IEnumerable<NewsArticle> GetNewsArticles()
        {
            using var context = new FUNewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .ToList();
        }

        public NewsArticle? GetNewsArticleById(string id)
        {
            using var context = new FUNewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .FirstOrDefault(c => c.NewsArticleID == id);
        }

        public void AddNewsArticle(NewsArticle article)
        {
            using var context = new FUNewsManagementContext();
            context.NewsArticles.Add(article);
            context.SaveChanges();
        }

        public void UpdateNewsArticle(NewsArticle article)
        {
            using var context = new FUNewsManagementContext();
            context.NewsArticles.Update(article);
            context.SaveChanges();
        }

        public void DeleteNewsArticle(NewsArticle article)
        {
            using var context = new FUNewsManagementContext();
            context.NewsArticles.Remove(article);
            context.SaveChanges();
        }

        public IEnumerable<NewsArticle> GetStatistics(System.DateTime startDate, System.DateTime endDate)
        {
            using var context = new FUNewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }
    }
}
