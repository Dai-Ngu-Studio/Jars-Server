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
        Task<IReadOnlyList<Bill>> GetAllBillByContractIdAsync(int? contractId);
        Task<IEnumerable<Bill>> GetAllBillAsync(string uid, string? searchName, string? sortOrder, int page, int size, DateTime? dateFrom, DateTime? dateTo);
        Task<Bill> GetBillByBillIdAsync(int? billId, string uid);
        Task UpdateBillAsync(Bill bill);
        Task CreateBillAsync(Bill bill);
        Task DeleteBillAsync(Bill bill);
    }
}
