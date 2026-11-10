using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly FUNewsManagementContext _context;

        public NewsArticleRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<NewsArticle> GetNewsArticles()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .ToList();
        }

        public NewsArticle? GetNewsArticleById(string id)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .FirstOrDefault(c => c.NewsArticleID == id);
        }

        public void AddNewsArticle(NewsArticle article)
        {
            _context.NewsArticles.Add(article);
            _context.SaveChanges();
        }

        public void UpdateNewsArticle(NewsArticle article)
        {
            _context.NewsArticles.Update(article);
            _context.SaveChanges();
        }

        public void DeleteNewsArticle(NewsArticle article)
        {
            _context.NewsArticles.Remove(article);
            _context.SaveChanges();
        }

        public IEnumerable<NewsArticle> GetStatistics(DateTime startDate, DateTime endDate)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }
    }
}
