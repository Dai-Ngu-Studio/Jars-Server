﻿using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class AccountManagement
    {
        private static AccountManagement instance = null;
        private static readonly object instanceLock = new object();
        private AccountManagement() { }
        public static AccountManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AccountManagement();
                    }
                    return instance;
                }
            }
        }

        public async Task<Account?> GetAsync(string id)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var account = await jarsDB.Accounts.FindAsync(id);
                return account;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetListAsync(int page, int size, string email, string displayName)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var accounts = await jarsDB.Accounts
                    .OrderBy(account => account.Email)
                    .Where(account => string.IsNullOrEmpty(account.Email) || account.Email.ToLower().Contains(email))
                    .Where(account => string.IsNullOrEmpty(account.DisplayName) || account.DisplayName.ToLower().Contains(displayName))
                    .Skip(page * size)
                    .Take(size)
                    .ToListAsync();
                return accounts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Account?> GetIncludedAsync(string id)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var account = await jarsDB.Accounts
                    .Include(account => account.Wallets).ThenInclude(wallet => wallet.CategoryWallets)
                    .Include(account => account.Wallets).ThenInclude(wallet => wallet.Transactions).ThenInclude(transaction => transaction.Note)
                    .Include(account => account.Contracts).ThenInclude(contract => contract.Bills).ThenInclude(bill => bill.BillDetails)
                    .Include(account => account.Contracts).ThenInclude(contract => contract.Bills)
                    .FirstOrDefaultAsync(account => account.Id.Equals(id));
                return account;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddAsync(Account account)
        {
            try
            {
                Account? _account = await GetAsync(account.Id);
                if (_account == null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Accounts.Add(account);
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified account already existed.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        public async Task UpdateAsync(Account account)
        {
            try
            {
                Account? _account = await GetAsync(account.Id);
                if (_account != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Entry<Account>(account).State = EntityState.Modified;
                    await jarsDB.SaveChangesAsync();
                } else
                {
                    throw new Exception("Specified account does not exist.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Account account)
        {
            try
            {
                Account? _account = await GetAsync(account.Id);
                if (_account != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Accounts.Remove(account);
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified account does not exist.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }
    }
}
