using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IWalletReposiotry
    {
        Task UpdateWallet(Wallet wallet);
        Task<Wallet> GetWallet(int id);
        Task DeleteWallet(int id);     
        Task<IEnumerable<Wallet>> GetAllWallets(string id);
        Task AddWallet(Wallet wallet);
        Task Add6DefaultJars(string id, decimal totalAmount);

    }
}
