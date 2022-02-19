using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IAccountDeviceRepository
    {
        public abstract Task<AccountDevice?> GetAsync(string fcmToken);
        public abstract Task AddAsync(AccountDevice accountDevice);
        public abstract Task UpdateAsync(AccountDevice accountDevice);
        public abstract Task DeleteAsync(AccountDevice accountDevice);
    }
}
