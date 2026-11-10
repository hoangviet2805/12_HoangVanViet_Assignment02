using System;
using System.Collections.Generic;
using System.Linq;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FUNewsManagementContext _context;

        public CategoryRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetCategories() => _context.Categories.ToList();

        public Category? GetCategoryById(short id) => _context.Categories.FirstOrDefault(c => c.CategoryID == id);

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            bool hasArticles = _context.NewsArticles.Any(n => n.CategoryID == category.CategoryID);
            if (hasArticles)
            {
                throw new InvalidOperationException("Cannot delete category because it has news articles.");
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}
