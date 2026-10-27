using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.DAOs
{
    /// <summary>
    /// Data Access Object cho Bài báo (NewsArticle)
    /// - Quản lý toàn bộ câu truy vấn liên quan đến bài viết thông qua Entity Framework Core.
    /// - Áp dụng Singleton Pattern với Thread-safe lock.
    /// </summary>
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance;
        private static readonly object _instanceLock = new object();

        // Khởi tạo private để ngăn việc new đối tượng tự do
        private NewsArticleDAO() { }

        /// <summary>
        /// Property trả về thể hiện duy nhất của NewsArticleDAO (Singleton Pattern)
        /// - Đảm bảo chỉ có 1 đối tượng duy nhất quản lý các thao tác truy cập bài viết.
        /// </summary>
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

        /// <summary>
        /// Lấy toàn bộ danh sách bài viết kèm các thông tin liên kết (Eager Loading)
        /// - Dùng .Include(n => n.Category) để nạp bảng Category.
        /// - Dùng .Include(n => n.CreatedBy) để nạp người tạo.
        /// - Dùng .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag) để nạp các thẻ Tag gắn với bài viết.
        /// </summary>
        public IEnumerable<NewsArticle> GetNewsArticles()
        {
            using var context = new FUNewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .ToList();
        }

        /// <summary>
        /// Lấy chi tiết một bài viết theo ID
        /// </summary>
        public NewsArticle? GetNewsArticleById(string id)
        {
            using var context = new FUNewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
                .FirstOrDefault(c => c.NewsArticleID == id);
        }

        /// <summary>
        /// Thêm bài viết mới vào CSDL
        /// - Thêm vào DbSet NewsArticles và gọi SaveChanges() để sinh lệnh INSERT SQL.
        /// </summary>
        public void AddNewsArticle(NewsArticle article)
        {
            using var context = new FUNewsManagementContext();
            context.NewsArticles.Add(article);
            context.SaveChanges();
        }

        /// <summary>
        /// Cập nhật thông tin bài viết
        /// </summary>
        public void UpdateNewsArticle(NewsArticle article)
        {
            using var context = new FUNewsManagementContext();
            context.NewsArticles.Update(article);
            context.SaveChanges();
        }

        /// <summary>
        /// Xóa bài viết khỏi CSDL
        /// </summary>
        public void DeleteNewsArticle(NewsArticle article)
        {
            using var context = new FUNewsManagementContext();
            context.NewsArticles.Remove(article);
            context.SaveChanges();
        }

        /// <summary>
        /// Báo cáo thống kê bài viết theo khoảng ngày (dành cho Admin)
        /// - Lọc theo điều kiện: CreatedDate >= startDate và CreatedDate <= endDate.
        /// - Sắp xếp giảm dần theo ngày tạo (OrderByDescending) để bài mới nhất lên đầu.
        /// </summary>
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
