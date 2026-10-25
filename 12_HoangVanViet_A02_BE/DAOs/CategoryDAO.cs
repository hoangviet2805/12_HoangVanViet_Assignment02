using System.Collections.Generic;
using System.Linq;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.DAOs
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _instanceLock = new object();

        private CategoryDAO() { }

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

        public IEnumerable<Category> GetCategories()
        {
            using var context = new FUNewsManagementContext();
            return context.Categories.ToList();
        }

        public Category? GetCategoryById(short id)
        {
            using var context = new FUNewsManagementContext();
            return context.Categories.FirstOrDefault(c => c.CategoryID == id);
        }

        public void AddCategory(Category category)
        {
            using var context = new FUNewsManagementContext();
            context.Categories.Add(category);
            context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            using var context = new FUNewsManagementContext();
            context.Categories.Update(category);
            context.SaveChanges();
        }

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
