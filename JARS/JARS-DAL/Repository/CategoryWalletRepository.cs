using JARS_DAL.DAO;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class CategoryWalletReposiotry : ICategoryWalletReposiotry
    {
        public void AddCategoryWallet(CategoryWallet CategoryWallet) => CategoryWalletManagement.Instance.AddCategoryWallet(CategoryWallet);
       


        public void DeleteCategoryWallet(int id)
       => CategoryWalletManagement.Instance.RemoveCategoryWallet(id);


        public IEnumerable<CategoryWallet> GetAllCategoryWallets()
      => CategoryWalletManagement.Instance.GetCategoryWallets();


        public CategoryWallet GetCategoryWallet(int id)
       => CategoryWalletManagement.Instance.GetCategoryWallet(id);

        public void UpdateCategoryWallet(CategoryWallet CategoryWallet)
        => CategoryWalletManagement.Instance.UpdateCategoryWallet(CategoryWallet);

        
    }
}
