using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class BillDetailManagement
    {
        private static BillDetailManagement instance = null;
        private static readonly object instanceLock = new object();

        private BillDetailManagement() { }
        public static BillDetailManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new BillDetailManagement();
                    }
                    return instance;
                }
            }
        }

        public async Task CreateBillDetailAsync(BillDetail billDetail)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.BillDetails.Add(billDetail);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        public async Task UpdateBillDetailAsync(BillDetail billDetail)
        {
            try
            {
                BillDetail detail = await GetBillDetailAsync(billDetail.Id);
                if (detail != null)
                {
                    BillDetail tmpDetail = new BillDetail
                    {
                        Id = billDetail.Id,
                        ItemName = billDetail.ItemName,
                        Price = billDetail.Price,
                        Quantity = billDetail.Quantity,
                        BillId = detail.BillId,
                    };
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.BillDetails.Update(tmpDetail);
                    await jarsDB.SaveChangesAsync();
                }         
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteBillDetailAsync(BillDetail billDetail)
        {
            try
            {
                BillDetail detail = await GetBillDetailAsync(billDetail.Id);
                if (detail != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.BillDetails.Remove(billDetail);
                    await jarsDB.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IReadOnlyList<BillDetail>> GetAllBillDetailWithBillIdAsync(int? billId)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.BillDetails
                .Where(bd => bd.BillId == billId)
                .ToListAsync();
        }
        public async Task<BillDetail> GetBillDetailAsync(int id)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.BillDetails
                .SingleOrDefaultAsync(bd => bd.Id == id);
        }
    }
}
