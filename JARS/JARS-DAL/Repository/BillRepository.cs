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
        public Task<IReadOnlyList<Bill>> GetBillsAsync() => BillManagement.Instance.GetAllBillAsync();
        public Task<Bill> GetBillByBillIdAsync(int billId) => BillManagement.Instance.GetBillByBillIdAsync(billId);
        public Task UpdateBillAsync(Bill bill) => BillManagement.Instance.UpdateBillAsync(bill);
        public Task CreateBillAsync(Bill bill) => BillManagement.Instance.CreateBillAsync(bill);
    }
}
