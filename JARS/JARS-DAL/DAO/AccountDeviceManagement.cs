using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class AccountDeviceManagement
    {
        private static AccountDeviceManagement? instance = null;
        private static readonly object instanceLock = new object();
        private AccountDeviceManagement() { }
        public static AccountDeviceManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AccountDeviceManagement();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<AccountDevice>> GetListOfAccountAsync(string accountId)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var accountDevices = await jarsDB.AccountDevices
                    .Where(accountDevice => accountDevice.AccountId == accountId)
                    .ToListAsync();
                return accountDevices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AccountDevice>> GetListFromTokensAsync(List<string> fcmTokens)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var accountDevices = await jarsDB.AccountDevices
                    .Where(accountDevice => fcmTokens.Any(fcmToken => fcmToken == accountDevice.FcmToken))
                    .ToListAsync();
                return accountDevices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AccountDevice?> GetAsync(string fcmToken)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var accountDevice = await jarsDB.AccountDevices.FindAsync(fcmToken);
                return accountDevice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddAsync(AccountDevice accountDevice)
        {
            try
            {
                AccountDevice? _account = await GetAsync(accountDevice.FcmToken);
                if (_account == null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.AccountDevices.Add(accountDevice);
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified account device already existed.");
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(AccountDevice accountDevice)
        {
            try
            {
                AccountDevice? _account = await GetAsync(accountDevice.FcmToken);
                if (_account != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Entry<AccountDevice>(accountDevice).State = EntityState.Modified;
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified account device does not exist.");
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(AccountDevice accountDevice)
        {
            try
            {
                AccountDevice? _account = await GetAsync(accountDevice.FcmToken);
                if (_account != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.AccountDevices.Remove(accountDevice);
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified account device does not exist.");
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(IEnumerable<AccountDevice?> accountDevices)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                foreach (var accountDevice in accountDevices)
                {
                    if (accountDevice != null)
                    {
                        AccountDevice? _account = await GetAsync(accountDevice.FcmToken);
                        if (_account != null)
                        {
                            jarsDB.AccountDevices.Remove(accountDevice);
                        }
                    }
                }
                await jarsDB.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
