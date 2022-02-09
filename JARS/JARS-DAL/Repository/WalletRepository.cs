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
        public void AddWallet(Wallet wallet) => WalletManagement.Instance.AddWallet(wallet);


        public void DeleteWallet(int id) => WalletManagement.Instance.RemoveWallet(id);
       

        public IEnumerable<Wallet> GetAllWallets() =>WalletManagement.Instance.GetWallets();

        

        public Wallet GetWallet(int id) =>WalletManagement.Instance.GetWallet(id);
      

        public void UpdateWallet(Wallet wallet) =>WalletManagement.Instance.UpdateWallet(wallet);
        
    }
}
