using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class ContractManagement
    {
        public const int DAILY = 1;
        public const int WEEKLY = 2;
        public const int MONTHLY = 3;

        private static ContractManagement instance = null;
        private static readonly object instanceLock = new object();

        private ContractManagement()
        {
        }

        public static ContractManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ContractManagement();
                    }

                    return instance;
                }
            }
        }

        public async Task<IEnumerable<Contract>> GetAllContractAsync(string uid, string? searchName, string? sortOrder,
            int page, int size)
        {
            var jarsDB = new JarsDatabaseContext();
            var contracts = await jarsDB.Contracts
                .Where(c => c.AccountId == uid)
                .ToListAsync();

            if (searchName != null)
            {
                contracts = contracts.Where(contract => contract.Name!.ToLower().Contains(searchName.ToLower()))
                    .ToList();
            }

            if (sortOrder != null)
            {
                switch (sortOrder)
                {
                    case "asc":
                        contracts = contracts.OrderBy(s => s.StartDate).ToList();
                        break;
                    case "desc":
                        contracts = contracts.OrderByDescending(s => s.StartDate).ToList();
                        break;
                }
            }

            return contracts.Skip(page * size)
                .Take(size);
        }

        public async Task<Contract> GetContractByContractIdAsync(int? id, string uid)
        {
            var jarsDB = new JarsDatabaseContext();
            return await jarsDB.Contracts.Include(c => c.Note)
                .SingleOrDefaultAsync(c => c.AccountId == uid && c.Id == id);
        }

        public async Task CreateContractAsync(Contract contract)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Contracts.Add(contract);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
        }

        public async Task UpdateContractAsync(Contract contract)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                jarsDB.Contracts.Update(contract);
                await jarsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Contract>> CreateBillByContract()
        {
            List<Contract> activeContracts;
            try
            {
                var jarsDB = new JarsDatabaseContext();
                activeContracts = jarsDB.Contracts
                    .Include(c => c.Account)
                    .ThenInclude(a => a.AccountDevices)
                    .Where(c => (c.EndDate >= DateTime.Now) && (c.StartDate <= DateTime.Now))
                    .ToList();
                foreach (var contract in activeContracts)
                {
                    DateTime startDate = contract.StartDate ?? DateTime.Now;
                    switch (contract.ScheduleTypeId)
                    {
                        case DAILY:
                            await AddBillWithContract(contract);
                            break;
                        case WEEKLY:
                            if (startDate.DayOfWeek == DateTime.Now.DayOfWeek)
                            {
                                await AddBillWithContract(contract);
                            }

                            break;
                        case MONTHLY:
                            if (startDate.Day == DateTime.Now.Day)
                            {
                                await AddBillWithContract(contract);
                            }

                            break;
                    }
                }
                // return await jarsDB.Contracts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            return activeContracts;
        }

        public async Task<IEnumerable<Contract>> CreateBillByContractDemo()
        {
            List<Contract> activeContracts;
            List<Contract> processcingContracts = new List<Contract>();
            try
            {
                var jarsDB = new JarsDatabaseContext();
                activeContracts = jarsDB.Contracts
                    .Include(c => c.Account)
                    .ThenInclude(a => a.AccountDevices)
                    .Where(c => (c.EndDate >= DateTime.Now) && (c.StartDate <= DateTime.Now) && c.ScheduleTypeId == 4)
                    .ToList();
                foreach (var contract in activeContracts)
                {
                    DateTime startDate = contract.StartDate ?? DateTime.Now;
                    if (!jarsDB.Bills.Any(b => b.Name == contract.Name))
                    {
                        processcingContracts.Add(contract);
                        await AddBillWithContract(contract);
                    }
                }
                // return await jarsDB.Contracts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            return processcingContracts;
        }

        private async Task AddBillWithContract(Contract contract)
        {
            Bill bill = new Bill
            {
                Amount = contract.Amount,
                Date = DateTime.Now,
                Name = contract.Name,
                LeftAmount = contract.Amount,
                ContractId = contract.Id,
                AccountId = contract.AccountId
            };
            await BillManagement.Instance.CreateBillAsync(bill);
        }
    }
}