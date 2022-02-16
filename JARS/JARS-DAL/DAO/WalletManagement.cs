using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<Wallet>> GetWallets(string id)
        {            
            try
            {
                var jarsDB = new  JarsDatabaseContext();
                return await jarsDB.Wallets.Where(wallet => wallet.AccountId == id).ToListAsync();

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public async Task<Wallet> GetWallet(int id)
        {
            
            try
            {
                var jarsDB = new JarsDatabaseContext();
                return await jarsDB.Wallets.FindAsync(id);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public async Task AddWallet(Wallet wallet)
        {   
            try
            {
                var jarDB = new JarsDatabaseContext();
                jarDB.Wallets.Add(wallet);
                await jarDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
        }
        public async Task UpdateWallet (Wallet wallet)
        {
            try
            {

                var jarDB = new JarsDatabaseContext();
                jarDB.Wallets.Update(wallet);
                await jarDB.SaveChangesAsync();
            }
            catch (Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task RemoveWallet(int id)
        {
            try
            {
                Wallet _wallet = await GetWallet(id);
                if(_wallet != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Wallets.Remove(_wallet);
                    await jarsDB.SaveChangesAsync();
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
