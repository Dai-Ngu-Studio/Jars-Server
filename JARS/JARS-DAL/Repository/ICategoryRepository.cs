using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByCategoryIdAsync(int categoryId);
        Task UpdateCategoryAsync(Category category);
        Task CreateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
}
