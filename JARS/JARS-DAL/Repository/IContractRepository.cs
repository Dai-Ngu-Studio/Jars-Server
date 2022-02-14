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
        Task<IReadOnlyList<Contract>> GetAllContractAsync();
        Task<Contract> GetContractByContractIdAsync(int id);
    }
}
