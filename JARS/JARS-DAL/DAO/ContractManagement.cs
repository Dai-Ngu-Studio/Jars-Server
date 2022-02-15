using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task<IReadOnlyList<Contract>> GetAllContractAsync()
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Contracts
                .ToListAsync();
        }
        public async Task<Contract> GetContractByContractIdAsync(int id)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Contracts
                .FindAsync(id);
        }
    }
}
