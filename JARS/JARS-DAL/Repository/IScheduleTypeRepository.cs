using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public interface IScheduleTypeRepository
    {
        public abstract Task<IEnumerable<ScheduleType>> GetListAsync();
        public abstract Task<ScheduleType?> GetAsync(int id);
        public abstract Task AddAsync(ScheduleType scheduleType);
        public abstract Task UpdateAsync(ScheduleType scheduleType);
        public abstract Task DeleteAsync(ScheduleType scheduleType);
    }
}
