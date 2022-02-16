using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var jarsDB = new JarsDatabaseContext();
                jarsDB.BillDetails.Update(billDetail);
                await jarsDB.SaveChangesAsync();
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
                var jarsDB = new JarsDatabaseContext();
                jarsDB.BillDetails.Remove(billDetail);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IReadOnlyList<BillDetail>> GetAllBillDetailWithBillIdAsync(int? billId, string uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.BillDetails
                .Where(bd => bd.BillId == billId && bd.Bill.Contract.AccountId == uid)
                .ToListAsync();
        }
        public async Task<BillDetail> GetBillDetailAsync(int id, string uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.BillDetails
                .SingleOrDefaultAsync(bd => bd.Id == id && bd.Bill.Contract.AccountId == uid);
        }
    }
}
