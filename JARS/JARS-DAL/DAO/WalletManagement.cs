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
                throw new Exception(ex.InnerException.Message);
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
                throw new Exception(ex.InnerException.Message);
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
                throw new Exception(ex.InnerException.Message);
            }
        }
        public async Task Add6DefaultJars(string id,decimal totalAmount)
        {
            try
            {
                var jarDB = new JarsDatabaseContext();
                CategoryWallet categoryWallet = new CategoryWallet()
                {
                    Name = "FromSalary",
                    CurrentCategoryLevel = 0,
                    
                };
                jarDB.CategoryWallets.Add(categoryWallet);
                jarDB.SaveChanges();
                categoryWallet.ParentCategoryId = categoryWallet.Id;
                jarDB.CategoryWallets.Update(categoryWallet);
                List<Wallet> wallets =  new List<Wallet>{ 
                    new Wallet()
                    { 

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "The Necessaries Account",
                        Percentage = 55,
                        StartDate = DateTime.Now,
                        WalletAmount = (totalAmount*55)/100,
                    },
                    new Wallet()
                    {

                            AccountId = id,
                            CategoryWalletId = categoryWallet.Id,
                            Name = "Financial Freedom Account",
                            Percentage = 10,
                            StartDate = DateTime.Now,
                            WalletAmount= (totalAmount *10)/100,
                    },
                    new Wallet()
                     {

                            AccountId = id,
                            CategoryWalletId = categoryWallet.Id,
                            Name = "Long-term Saving Account",
                            Percentage = 10,
                            StartDate = DateTime.Now,
                            WalletAmount= (totalAmount * 10) / 100,
                    },
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Education Account",
                        Percentage = 10,
                        StartDate = DateTime.Now,
                        WalletAmount= (totalAmount*10)/100,
                    },
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Play Account",
                        Percentage = 10,
                        StartDate = DateTime.Now,
                        WalletAmount= (totalAmount*10)/100,
                    },
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Give Account",
                        Percentage = 5,
                        StartDate = DateTime.Now
                        ,WalletAmount = (totalAmount*5)/100,
                    },

                };
                jarDB.Wallets.AddRange(wallets);
                await jarDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
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
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
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
                throw new Exception(ex.InnerException.Message);
            }
        }
    }
}
