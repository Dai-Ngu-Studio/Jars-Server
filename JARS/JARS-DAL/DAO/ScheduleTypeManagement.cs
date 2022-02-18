using JARS_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.DAO
{
    public class ScheduleTypeManagement
    {
        private static ScheduleTypeManagement? instance = null;
        private static readonly object instanceLock = new object();
        public static ScheduleTypeManagement Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ScheduleTypeManagement();
                    }
                    return instance;
                }
            }
        }

        public async Task<ScheduleType?> GetAsync(int id)
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var scheduleType = await jarsDB.ScheduleTypes.FindAsync(id);
                return scheduleType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScheduleType>> GetListAsync()
        {
            try
            {
                var jarsDB = new JarsDatabaseContext();
                var scheduleTypes = await jarsDB.ScheduleTypes.ToListAsync();
                return scheduleTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddAsync(ScheduleType scheduleType)
        {
            try
            {
                ScheduleType? _scheduleType = await GetAsync(scheduleType.Id);
                if (_scheduleType == null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.ScheduleTypes.Add(scheduleType);
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified schedule type already existed.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateAsync(ScheduleType scheduleType)
        {
            try
            {
                ScheduleType? _scheduleType = await GetAsync(scheduleType.Id);
                if (_scheduleType != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.Entry<ScheduleType>(scheduleType).State = EntityState.Modified;
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified schedule type does not exist.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(ScheduleType scheduleType)
        {
            try
            {
                ScheduleType? _scheduleType = await GetAsync(scheduleType.Id);
                if (_scheduleType != null)
                {
                    var jarsDB = new JarsDatabaseContext();
                    jarsDB.ScheduleTypes.Remove(scheduleType);
                    await jarsDB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Specified schedule type does not exist.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
