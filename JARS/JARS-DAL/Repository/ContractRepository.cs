using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class ContractRepository : IContractRepository
    {
        public Task<IReadOnlyList<Contract>> GetAllContractAsync(string uid) => ContractManagement.Instance.GetAllContractAsync(uid);
        public Task<Contract> GetContractByContractIdAsync(int? id, string uid) => ContractManagement.Instance.GetContractByContractIdAsync(id, uid);
        public Task CreateContractAsync(Contract contract) => ContractManagement.Instance.CreateContractAsync(contract);
        public Task UpdateContractAsync(Contract contract) => ContractManagement.Instance.UpdateContractAsync(contract);
        public Task<IEnumerable<Contract>> GetActiveContractsAsync(string uid) => ContractManagement.Instance.GetActiveContractsAsync(uid);
    }
}
