using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class WalletRepository : IWalletReposiotry
    {
        public Task Add6DefaultJars(string id, decimal totalAmount) =>WalletManagement.Instance.Add6DefaultJars(id, totalAmount);
       

        public Task AddWallet(Wallet wallet) => WalletManagement.Instance.AddWallet(wallet);

        public Task<int> countWallets(string uid) => WalletManagement.Instance.countWalletsByUserID(uid);
  

        public Task DeleteWallet(int id) => WalletManagement.Instance.RemoveWallet(id);
       

        public Task<IEnumerable<Wallet>> GetAllWallets(string id) =>WalletManagement.Instance.GetWallets(id);

        

        public Task<Wallet> GetWallet(int id) =>WalletManagement.Instance.GetWallet(id);
      

        public Task UpdateWallet(Wallet wallet) =>WalletManagement.Instance.UpdateWallet(wallet);

       
    }
}
