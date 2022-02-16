using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class BillDetailRepository : IBillDetailRepository
    {
        public Task CreateBillDetailAsync(BillDetail billDetail) => BillDetailManagement.Instance.CreateBillDetailAsync(billDetail);

        public Task DeleteBillDetailAsync(BillDetail billDetail) => BillDetailManagement.Instance.DeleteBillDetailAsync(billDetail);
        public Task<IReadOnlyList<BillDetail>> GetAllBillDetailWithBillIdAsync(int? billId, string uid) => BillDetailManagement.Instance.GetAllBillDetailWithBillIdAsync(billId, uid);

        public Task<BillDetail> GetBillDetailAsync(int id, string uid) => BillDetailManagement.Instance.GetBillDetailAsync(id, uid);
        public Task UpdateBillDetailAsync(BillDetail billDetail) => BillDetailManagement.Instance.UpdateBillDetailAsync(billDetail);

    }
}
