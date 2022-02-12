using JARS_DAL.Models;
using System;
using System.Collections.Generic;
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
        public IEnumerable<CategoryWallet> GetCategoryWallets()
        {   List<CategoryWallet> CategoryWallets;
            try
            {
                var jarsDB = new  JarsDatabaseContext();
                CategoryWallets = jarsDB.CategoryWallets.ToList();

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return CategoryWallets;
        }
        public CategoryWallet GetCategoryWallet(int id)
        {
            CategoryWallet CategoryWallet =null;
            try
            {
                var jarsDB = new JarsDatabaseContext();
                CategoryWallet = jarsDB.CategoryWallets.SingleOrDefault(CategoryWallet => CategoryWallet.Id == id);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return CategoryWallet;
        }
        public void AddCategoryWallet(CategoryWallet CategoryWallet)
        {   
            try
            {
                CategoryWallet _CategoryWallet = GetCategoryWallet(CategoryWallet.Id);
                if(_CategoryWallet == null)
                {
                    var jardDB = new JarsDatabaseContext();
                    jardDB.CategoryWallets.Add(CategoryWallet);
                    jardDB.SaveChanges();
                }
                else
                {
                    throw new Exception("CategoryWallet already exist");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
        }
        public void UpdateCategoryWallet (CategoryWallet CategoryWallet)
        {
            try
            {
                CategoryWallet _CategoryWallet = GetCategoryWallet(CategoryWallet.Id);
                if(_CategoryWallet != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Entry<CategoryWallet>(CategoryWallet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    jarsDB.SaveChanges();
                }
                else
                {
                    throw new Exception("this CategoryWallet does not already existed");
                }
            }
            catch (Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }
        public void RemoveCategoryWallet(int id)
        {
            try
            {
                CategoryWallet _CategoryWallet = GetCategoryWallet(id);
                if(_CategoryWallet != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.CategoryWallets.Remove(_CategoryWallet);
                    jarsDB.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
