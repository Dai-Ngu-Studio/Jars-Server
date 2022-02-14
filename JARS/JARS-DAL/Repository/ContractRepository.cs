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
        public Task<IReadOnlyList<Contract>> GetAllContractAsync() => ContractManagement.Instance.GetAllContractAsync();
        public Task<Contract> GetContractByContractIdAsync(int id) => ContractManagement.Instance.GetContractByContractIdAsync(id) ;
    }
}
