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
        Task<IEnumerable<Contract>> GetAllContractAsync(string uid, string? searchName, string? sortOrder, int page, int size);
        Task<Contract> GetContractByContractIdAsync(int? id, string uid);
        Task CreateContractAsync(Contract contract);
        Task UpdateContractAsync(Contract contract);
        Task<IEnumerable<Contract>> CreateBillByContract();
        Task<IEnumerable<Contract>> CreateBillByContractDemo();
    }
}
