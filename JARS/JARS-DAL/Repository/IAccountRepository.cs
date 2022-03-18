using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IAccountRepository
    {
        public abstract Task<IEnumerable<Account>> GetListAsync(int page, int size, string search);
        public abstract Task<int> GetTotalAccount(String search);
        public abstract Task<Account?> GetAsync(string id);
        public abstract Task<Account?> GetIncludedAsync(string id);
        public abstract Task AddAsync(Account account);
        public abstract Task UpdateAsync(Account account);
        public abstract Task DeleteAsync(Account account);
    }
}
