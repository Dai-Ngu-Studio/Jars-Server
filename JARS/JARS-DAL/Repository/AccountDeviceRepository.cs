using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class AccountDeviceRepository : IAccountDeviceRepository
    {
        public async Task<AccountDevice?> GetAsync(string fcmToken) => await AccountDeviceManagement.Instance.GetAsync(fcmToken);
        public async Task<IEnumerable<AccountDevice?>> GetListFromTokensAsync(List<string> fcmTokens) =>
            await AccountDeviceManagement.Instance.GetListFromTokensAsync(fcmTokens);
        public async Task AddAsync(AccountDevice accountDevice) => await AccountDeviceManagement.Instance.AddAsync(accountDevice);
        public async Task UpdateAsync(AccountDevice accountDevice) => await AccountDeviceManagement.Instance.UpdateAsync(accountDevice);
        public async Task DeleteAsync(AccountDevice accountDevice) => await AccountDeviceManagement.Instance.DeleteAsync(accountDevice);
        public async Task DeleteListAsync(IEnumerable<AccountDevice?> accountDevices) => await AccountDeviceManagement.Instance.DeleteListAsync(accountDevices);

    }
}
