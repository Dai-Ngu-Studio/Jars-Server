using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IContractRepository
    {
        Task<IReadOnlyList<Contract>> GetAllContractAsync(string uid);
        Task<Contract> GetContractByContractIdAsync(int? id, string uid);
        Task CreateContractAsync(Contract contract);
        Task UpdateContractAsync(Contract contract);
    }
}
