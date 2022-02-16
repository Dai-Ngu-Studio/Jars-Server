using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class ContractManagement
    {
        private static ContractManagement instance = null;
        private static readonly object instanceLock = new object();

        private ContractManagement() { }
        public static ContractManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ContractManagement();
                    }
                    return instance;
                }
            }
        }
        public async Task<IReadOnlyList<Contract>> GetAllContractAsync(string uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Contracts
                .Where(c => c.AccountId == uid)
                .ToListAsync();
        }
        public async Task<Contract> GetContractByContractIdAsync(int id, string uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Contracts
                .SingleOrDefaultAsync(c => c.AccountId == uid && c.Id == id);
        }

        public async Task CreateContractAsync(Contract contract)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Contracts.Add(contract);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        public async Task UpdateContractAsync(Contract contract)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Contracts.Update(contract);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
