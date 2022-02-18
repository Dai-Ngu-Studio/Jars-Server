using JARS_DAL.DAO;
using JARS_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Repository
{
    public class ScheduleTypeRepository : IScheduleTypeRepository
    {
        public async Task<IEnumerable<ScheduleType>> GetListAsync() => await ScheduleTypeManagement.Instance.GetListAsync();
        public async Task<ScheduleType?> GetAsync(int id) => await ScheduleTypeManagement.Instance.GetAsync(id);
        public async Task AddAsync(ScheduleType scheduleType) => await ScheduleTypeManagement.Instance.AddAsync(scheduleType);
        public async Task UpdateAsync(ScheduleType scheduleType) => await ScheduleTypeManagement.Instance.UpdateAsync(scheduleType);
        public async Task DeleteAsync(ScheduleType scheduleType) => await ScheduleTypeManagement.Instance.DeleteAsync(scheduleType);
    }
}
