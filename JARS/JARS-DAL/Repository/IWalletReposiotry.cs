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
        void UpdateWallet(Wallet wallet);
        Wallet GetWallet(int id);
        void DeleteWallet(int id);
        
        IEnumerable<Wallet> GetAllWallets();
        void AddWallet(Wallet wallet);

    }
}
