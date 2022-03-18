using JARS_DAL.DAO;
using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace JARS_DAL.Repository
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<IEnumerable<Account>> GetListAsync(int page, int size, string search) => await AccountManagement.Instance.GetListAsync(page, size, search);
        public async Task<Account?> GetAsync(string id) => await AccountManagement.Instance.GetAsync(id);
        public async Task<Account?> GetIncludedAsync(string id) => await AccountManagement.Instance.GetIncludedAsync(id);
        public async Task AddAsync(Account account) => await AccountManagement.Instance.AddAsync(account);
        public async Task UpdateAsync(Account account) => await AccountManagement.Instance.UpdateAsync(account);
        public async Task DeleteAsync(Account account) => await AccountManagement.Instance.DeleteAsync(account);
        public async Task<int> GetTotalAccount(String search) => await AccountManagement.Instance.GetTotalAccount(search);   
    }
}
