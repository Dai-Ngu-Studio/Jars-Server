using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class BillManagement
    {
        private static BillManagement instance = null;
        private static readonly object instanceLock = new object();

        private BillManagement() { }
        public static BillManagement Instance { 
            get {
                lock (instanceLock) {
                    if (instance == null) { 
                        instance = new BillManagement();
                    }
                    return instance;
                }
            } 
        }
        public async Task<IReadOnlyList<Bill>> GetAllBillByContractIdAsync(int? contractId)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Bills
                .Where(b => b.ContractId == contractId)
                .ToListAsync(); 
        }

        public async Task<IEnumerable<Bill>> GetAllBillAsync
            (string uid, string? searchName, string? sortOrder, int page, int size, DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var bills = await jarsDB.Bills
                    .Where(s => s.AccountId == uid)
                    .Skip(page * size)
                    .Take(size)
                    .ToListAsync();
                if (searchName != null)
                {
                    bills = bills.Where(bill => bill.Name!.ToLower().Contains(searchName.ToLower())).ToList();
                }
                if (dateFrom.HasValue && dateTo.HasValue)
                {
                    if (DateTime.Compare((DateTime)dateFrom, (DateTime)dateTo) > 0)
                    {
                        DateTime? temp = dateFrom;
                        dateFrom = dateTo;
                        dateTo = temp;
                    }
                    bills = bills.Where(bill => DateTime.Compare(bill.Date!.Value.Date, dateFrom.Value.Date) >= 0
                            && DateTime.Compare(bill.Date.Value.Date, dateTo.Value.Date) <= 0).ToList();
                } 

                switch (sortOrder)
                {
                    case "asc":
                        bills = bills.OrderBy(s => s.Date).ToList();
                        break;
                    case "desc":
                        bills = bills.OrderByDescending(s => s.Date).ToList();
                        break;
                    case "z-a":
                        bills = bills.OrderByDescending(s => s.Name).ToList();
                        break;
                    default:
                        bills = bills.OrderBy(s => s.Name).ToList();
                        break;
                }
                return bills;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }    
        }

        public async Task<Bill> GetBillByBillIdAsync (int? id, string uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Bills
                .Include(bdt => bdt.BillDetails)
                .FirstOrDefaultAsync(b => b.Id == id && b.AccountId == uid);
        }

        public async Task UpdateBillAsync(Bill bill)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Bills.Update(bill);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateBillAsync(Bill bill)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Bills.Add(bill);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        public async Task DeleteBillAsync(Bill bill)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Bills.Remove(bill);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
