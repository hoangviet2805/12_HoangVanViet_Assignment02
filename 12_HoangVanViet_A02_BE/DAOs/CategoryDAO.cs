using System.Collections.Generic;
using System.Linq;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.DAOs
{
    /// <summary>
    /// Data Access Object cho Danh mục (Category)
    /// - Quản lý việc truy vấn và cập nhật dữ liệu bảng Category thông qua EF Core (FUNewsManagementContext).
    /// - Áp dụng Singleton Pattern để tối ưu bộ nhớ và kiểm soát số lượng kết nối.
    /// </summary>
    public class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _instanceLock = new object();

        // Khởi tạo private để ngăn việc tạo new CategoryDAO() từ bên ngoài
        private CategoryDAO() { }

        /// <summary>
        /// Property trả về thể hiện duy nhất của CategoryDAO (Singleton Pattern - Thread-Safe)
        /// - Dùng lock (_instanceLock) để đảm bảo không bị xung đột khi nhiều luồng cùng gọi lúc khởi tạo.
        /// </summary>
        public static CategoryDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryDAO();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh mục trong CSDL
        /// </summary>
        public IEnumerable<Category> GetCategories()
        {
            using var context = new FUNewsManagementContext();
            return context.Categories.ToList();
        }

        /// <summary>
        /// Tìm danh mục theo ID
        /// </summary>
        public Category? GetCategoryById(short id)
        {
            using var context = new FUNewsManagementContext();
            return context.Categories.FirstOrDefault(c => c.CategoryID == id);
        }

        /// <summary>
        /// Thêm danh mục mới vào CSDL
        /// </summary>
        public void AddCategory(Category category)
        {
            using var context = new FUNewsManagementContext();
            context.Categories.Add(category);
            context.SaveChanges();
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        public void UpdateCategory(Category category)
        {
            using var context = new FUNewsManagementContext();
            context.Categories.Update(category);
            context.SaveChanges();
        }

        /// <summary>
        /// Xóa danh mục
        /// - RÀNG BUỘC NGHIỆP VỤ: Kiểm tra xem danh mục này có bài viết nào đang dùng không.
        /// - Nếu có (hasArticles == true): Ném ra ngoại lệ InvalidOperationException để ngăn xóa.
        /// - Nếu không: Thực hiện xóa và lưu thay đổi xuống Database.
        /// </summary>
        public void DeleteCategory(Category category)
        {
            using var context = new FUNewsManagementContext();
            bool hasArticles = context.NewsArticles.Any(n => n.CategoryID == category.CategoryID);
            if (hasArticles)
            {
                throw new System.InvalidOperationException("Cannot delete category because it has news articles.");
            }
            context.Categories.Remove(category);
            context.SaveChanges();
        }
    }
}
