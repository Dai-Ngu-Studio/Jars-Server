using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IBillDetailRepository
    {
        Task CreateBillDetailAsync(BillDetail billDetail);
        Task<BillDetail> GetBillDetailAsync(int id);
        Task<IReadOnlyList<BillDetail>> GetAllBillDetailWithBillIdAsync(int? billId);
        Task DeleteBillDetailAsync(BillDetail billDetail);
        Task UpdateBillDetailAsync(BillDetail billDetail);
    }
}
