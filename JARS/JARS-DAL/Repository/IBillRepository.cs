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
        Task<IReadOnlyList<Bill>> GetBillsAsync();
        Task<Bill> GetBillByBillIdAsync(int billId);
        Task UpdateBillAsync(Bill bill);
        Task CreateBillAsync(Bill bill);
        Task DeleteBillAsync(Bill bill);
    }
}
