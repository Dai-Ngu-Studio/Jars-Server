﻿using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task<IReadOnlyList<Bill>> GetAllBillAsync()
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Bills
                .Include(bdt => bdt.BillDetails)
                .Include(cate => cate.Category)
                .ToListAsync(); 
        }
        public async Task<Bill> GetBillByBillIdAsync (int id)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Bills
                .Include(bdt => bdt.BillDetails)
                .Include(bdt => bdt.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
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
                throw new Exception(ex.Message);
            }
        }
    }
}
