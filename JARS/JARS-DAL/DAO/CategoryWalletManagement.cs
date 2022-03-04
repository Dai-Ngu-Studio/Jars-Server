using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class CategoryWalletManagement
    {
        private static CategoryWalletManagement instance;
        private static readonly object instanceLock = new object();
        private CategoryWalletManagement()
        {

        }
        public static CategoryWalletManagement Instance
        {
            get { 
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryWalletManagement();
                    }
                }
                return instance; 
            }
        }
        public async Task<IEnumerable<CategoryWallet>> GetCategoryWallets()
        {   
            try
            {
                var jarsDB = new  JarsDatabaseContext();
                return await jarsDB.CategoryWallets.ToListAsync();
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }

        }
        public async Task<CategoryWallet> GetCategoryWallet(int id)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
               return await jarsDB.CategoryWallets.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }        
        }
        public async  Task AddCategoryWallet(CategoryWallet CategoryWallet)
        {   
            try
            {
                var jarDB = new JarsDatabaseContext();
                if(CategoryWallet.ParentCategoryId == 0)
                {
                    CategoryWallet.ParentCategoryId = null;
                }
                
                //CategoryWallet.ParentCategoryId = CategoryWallet.Id == 0 ?null:CategoryWallet.ParentCategoryId;
                //Console.WriteLine("new DBcontext");
                //Console.WriteLine("ID trươc add: " + CategoryWallet.Id + " Name: " + CategoryWallet.Name + " ParentCategoryId :" + CategoryWallet.ParentCategoryId + " CurrentCategoryId: " + CategoryWallet.CurrentCategoryLevel);
                jarDB.CategoryWallets.Add(CategoryWallet);
               // Console.WriteLine("addCategoryWallet");
                await jarDB.SaveChangesAsync();
                //Console.WriteLine("Save changes");
                //Console.WriteLine("ID sau add : " + CategoryWallet.Id + " Name: " + CategoryWallet.Name + " ParentCategoryId :" + CategoryWallet.ParentCategoryId + " CurrentCategoryId: " + CategoryWallet.CurrentCategoryLevel);
                if(CategoryWallet.ParentCategoryId is null)
                {
                    int cateNum = await jarDB.CategoryWallets.CountAsync();
                    CategoryWallet categoryWallet = new CategoryWallet
                    {
                        Id = CategoryWallet.Id,
                        Name = CategoryWallet.Name,
                        ParentCategoryId = cateNum == 1 ? CategoryWallet.Id : CategoryWallet.ParentCategoryId is null ? CategoryWallet.Id : CategoryWallet.ParentCategoryId,
                        CurrentCategoryLevel = CategoryWallet.CurrentCategoryLevel

                    };
                    //Console.WriteLine("ID sau update: " + categoryWallet.Id + " Name: " + categoryWallet.Name + " ParentCategoryId :" + categoryWallet.ParentCategoryId + " CurrentCategoryId: " + categoryWallet.CurrentCategoryLevel);
                    await UpdateCategoryWallet(categoryWallet);
                }       
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }
        public async Task UpdateCategoryWallet (CategoryWallet CategoryWallet)
        {
            try
            {
                var jarDB = new JarsDatabaseContext();
                jarDB.CategoryWallets.Update(CategoryWallet);
                await jarDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }
        public async Task RemoveCategoryWallet(int id)
        {
            try
            {
                CategoryWallet categoryWallet = await GetCategoryWallet(id);
                if(categoryWallet != null)
                {
                    var jarDB = new JarsDatabaseContext();
                    jarDB.CategoryWallets.Remove(categoryWallet);
                    await jarDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Category walet not found");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }
    }
}
