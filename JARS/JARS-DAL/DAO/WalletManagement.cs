using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class WalletManagement
    {
        private static WalletManagement instance;
        private static readonly object instanceLock = new object();
        private WalletManagement()
        {

        }
        public static WalletManagement Instance
        {
            get { 
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new WalletManagement();
                    }
                }
                return instance; 
            }
        }
        public IEnumerable<Wallet> GetWallets()
        {   List<Wallet> wallets;
            try
            {
                var jarsDB = new  JarsDatabaseContext();
                wallets = jarsDB.Wallets.ToList();

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return wallets;
        }
        public Wallet GetWallet(int id)
        {
            Wallet wallet =null;
            try
            {
                var jarsDB = new JarsDatabaseContext();
                wallet = jarsDB.Wallets.SingleOrDefault(wallet => wallet.Id == id);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return wallet;
        }
        public void AddWallet(Wallet wallet)
        {   
            try
            {
                Wallet _wallet = GetWallet(wallet.Id);
                if(_wallet == null)
                {
                    var jardDB = new JarsDatabaseContext();
                    jardDB.Wallets.Add(wallet);
                    jardDB.SaveChanges();
                }
                else
                {
                    throw new Exception("Wallet already existed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
        }
        public void UpdateWallet (Wallet wallet)
        {
            try
            {
                Wallet _wallet = GetWallet(wallet.Id);
                if( _wallet != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Entry<Wallet>(wallet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    jarsDB.SaveChanges();
                }
                else
                {
                    throw new Exception("this wallet does not already existed");
                }
            }
            catch (Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }
        public void RemoveWallet(int id)
        {
            try
            {
                Wallet _wallet = GetWallet(id);
                if(_wallet != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Wallets.Remove(_wallet);
                    jarsDB.SaveChanges();
                }
                else
                {
                    throw new Exception("This wallet doest not exist");
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
