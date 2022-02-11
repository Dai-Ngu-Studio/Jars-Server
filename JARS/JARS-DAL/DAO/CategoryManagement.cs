using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{   
    public class CategoryManagement
    {
        private static CategoryManagement instance = null;
        private static readonly object instanceLock = new object();

        private CategoryManagement() { }
        public static CategoryManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryManagement();
                    }
                    return instance;
                }
            }
        }
        public async Task<IReadOnlyList<Category>> GetAllCategoryAsync()
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Categories
                .ToListAsync();
        }
        public async Task<Category> GetCategoryByCatrgoryIdAsync(int id)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Categories
                .SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Categories.Update(category);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateCategoryAsync(Category category)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Categories.Add(category);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task DeleteCategoryAsync(Category category)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Categories.Remove(category);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
