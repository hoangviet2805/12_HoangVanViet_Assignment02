using System.Collections.Generic;
using _12_HoangVanViet_A02_BE.DAOs;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public void AddCategory(Category category) => CategoryDAO.Instance.AddCategory(category);
        public void DeleteCategory(Category category) => CategoryDAO.Instance.DeleteCategory(category);
        public IEnumerable<Category> GetCategories() => CategoryDAO.Instance.GetCategories();
        public Category? GetCategoryById(short id) => CategoryDAO.Instance.GetCategoryById(id);
        public void UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);
    }
}
