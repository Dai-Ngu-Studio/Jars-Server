using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JARS_DAL.Models;
namespace JARS_DAL.Repository
{
    public interface ICategoryWalletReposiotry
    {
        void UpdateCategoryWallet(CategoryWallet CategoryWallet);
        CategoryWallet GetCategoryWallet(int id);
        void DeleteCategoryWallet(int id);
        
        IEnumerable<CategoryWallet> GetAllCategoryWallets();
        void AddCategoryWallet(CategoryWallet CategoryWallet);

    }
}
