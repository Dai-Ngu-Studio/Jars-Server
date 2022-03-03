using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class BillRepository : IBillRepository
    {
        public Task<IReadOnlyList<Bill>> GetAllBillByContractIdAsync(int? contractId) => BillManagement.Instance.GetAllBillByContractIdAsync(contractId);
        public Task<IEnumerable<Bill>> GetAllBillAsync
            (string uid, string? searchName, string? sortOrder, int page, int size, DateTime? dateFrom, DateTime? dateTo)
            => BillManagement.Instance.GetAllBillAsync(uid, searchName, sortOrder, page, size, dateFrom, dateTo);
        public Task<Bill> GetBillByBillIdAsync(int? billId, string uid) => BillManagement.Instance.GetBillByBillIdAsync(billId, uid);
        public Task UpdateBillAsync(Bill bill) => BillManagement.Instance.UpdateBillAsync(bill);
        public Task CreateBillAsync(Bill bill) => BillManagement.Instance.CreateBillAsync(bill);
        public Task DeleteBillAsync(Bill bill) => BillManagement.Instance.DeleteBillAsync(bill);
    }
}
