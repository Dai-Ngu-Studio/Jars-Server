using JARS_DAL.DAO;
using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace JARS_DAL.Repository
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<Account?> GetAsync(string id) => await AccountDAO.Instance.GetAsync(id);
        public async Task AddAsync(Account account) => await AccountDAO.Instance.AddAsync(account);
        public async Task UpdateAsync(Account account) => await AccountDAO.Instance.UpdateAsync(account);
        public async Task DeleteAsync(Account account) => await AccountDAO.Instance.DeleteAsync(account);
    }
}
