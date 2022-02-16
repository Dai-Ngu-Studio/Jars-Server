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
        Task UpdateCategoryWallet(CategoryWallet CategoryWallet);
        Task<CategoryWallet> GetCategoryWallet(int id);
        Task DeleteCategoryWallet(int id);
        
        Task<IEnumerable<CategoryWallet>> GetAllCategoryWallets();
        Task AddCategoryWallet(CategoryWallet CategoryWallet);

    }
}
