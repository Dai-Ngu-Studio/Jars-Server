using JARS_DAL.Models;
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
