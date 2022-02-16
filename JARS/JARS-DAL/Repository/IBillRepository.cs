using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IBillRepository
    {
        Task<IReadOnlyList<Bill>> GetBillByContractIdAsync(int contractId, string uid);
        Task<Bill> GetBillByBillIdAsync(int billId, string uid);
        Task UpdateBillAsync(Bill bill);
        Task CreateBillAsync(Bill bill);
        Task DeleteBillAsync(Bill bill);
    }
}
