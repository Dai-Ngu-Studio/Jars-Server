using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<IReadOnlyList<Category>> GetCategoriesAsync() => CategoryManagement.Instance.GetAllCategoryAsync();
        public Task<Category> GetCategoryByCategoryIdAsync(int? categoryId) => CategoryManagement.Instance.GetCategoryByCatergoryIdAsync(categoryId);
        public Task UpdateCategoryAsync(Category category) => CategoryManagement.Instance.UpdateCategoryAsync(category);
        public Task CreateCategoryAsync(Category category) => CategoryManagement.Instance.CreateCategoryAsync(category);
        public Task DeleteCategoryAsync(Category category) => CategoryManagement.Instance.DeleteCategoryAsync(category);
    }
}
