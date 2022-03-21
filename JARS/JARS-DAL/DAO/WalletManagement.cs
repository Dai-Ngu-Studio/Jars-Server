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
        public async Task<int> countWalletsByUserID(String uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Wallets.Where(w => w.AccountId == uid).CountAsync();
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
                await jarDB.SaveChangesAsync();
                List<Wallet> wallets = new List<Wallet>{
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Necessities",
                        Percentage = 55,
                        StartDate = DateTime.Now,
                        WalletAmount = (totalAmount*55)/100,
                        
                    },
                    new Wallet()
                    {

                            AccountId = id,
                            CategoryWalletId = categoryWallet.Id,
                            Name = "Investment",
                            Percentage = 10,
                            StartDate = DateTime.Now,
                            WalletAmount= (totalAmount *10)/100,
                    },
                    new Wallet()
                     {

                            AccountId = id,
                            CategoryWalletId = categoryWallet.Id,
                            Name = "Saving",
                            Percentage = 10,
                            StartDate = DateTime.Now,
                            WalletAmount= (totalAmount * 10) / 100,
                    },
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Education",
                        Percentage = 10,
                        StartDate = DateTime.Now,
                        WalletAmount= (totalAmount*10)/100,
                    },
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Play",
                        Percentage = 10,
                        StartDate = DateTime.Now,
                        WalletAmount= (totalAmount*10)/100,
                    },
                    new Wallet()
                    {

                        AccountId = id,
                        CategoryWalletId = categoryWallet.Id,
                        Name = "Give",
                        Percentage = 5,
                        StartDate = DateTime.Now
                        ,WalletAmount = (totalAmount*5)/100,
                    },

                };
                categoryWallet.ParentCategoryId = categoryWallet.Id;
                jarDB.CategoryWallets.Update(categoryWallet);
                await jarDB.SaveChangesAsync();
               
                jarDB.Wallets.AddRange(wallets);
                await jarDB.SaveChangesAsync();
                 List<Transaction> transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        WalletId =  wallets[0].Id,
                        TransactionDate = DateTime.Now,
                        Amount = wallets[0].WalletAmount
                    },
                    new Transaction()
                    {
                        WalletId =  wallets[1].Id,
                        TransactionDate = DateTime.Now,
                        Amount = wallets[1].WalletAmount
                    },
                    new Transaction()
                    {
                        WalletId =  wallets[2].Id,
                        TransactionDate = DateTime.Now,
                        Amount = wallets[2].WalletAmount
                    },
                    new Transaction()
                    {
                        WalletId =  wallets[3].Id,
                        TransactionDate = DateTime.Now,
                        Amount = wallets[3].WalletAmount
                    },
                    new Transaction()
                    {
                        WalletId =  wallets[4].Id,
                        TransactionDate = DateTime.Now,
                        Amount = wallets[4].WalletAmount
                    },
                    new Transaction()
                    {
                        WalletId =  wallets[5].Id,
                        TransactionDate = DateTime.Now,
                        Amount = wallets[5].WalletAmount                       
                    }
                };

                jarDB.Transactions.AddRange(transactions);
                await jarDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }
        public async Task<TransactionWallet> GetSpendOfAWallet(int id)
        {
            var jarDB = new JarsDatabaseContext();
            List<Transaction> tranQuery= new List<Transaction>();
            TransactionWallet transactionWallet = new TransactionWallet();
            transactionWallet.totalAdded = 0;
            transactionWallet.totalSpend = 0;
           
            try
            {
                tranQuery = await jarDB.Transactions.Where(t => t.WalletId == id).ToListAsync();
                if(tranQuery.Count() > 0) {
                    transactionWallet.Id = id;
                    transactionWallet.walletName = GetWallet(id).Result.Name;
                    foreach (var trans in tranQuery)
                    {
                        if (trans.Amount != null && trans.Amount > 0)
                        {
                            transactionWallet.totalAdded += trans.Amount;                          
                        }
                        else if (trans.Amount < 0)
                        {
                            transactionWallet.totalSpend += trans.Amount;
                        }

                        if (transactionWallet.totalAdded == null || transactionWallet.totalAdded == 0)
                        {
                            transactionWallet.totalAdded = 0;
                        }
                        else if (transactionWallet.totalSpend == null || transactionWallet.totalAdded == 0)
                        {
                            transactionWallet.totalSpend = 0;
                        }
                    }
                }
              
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message);
            }
            return transactionWallet;
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
