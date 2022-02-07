using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class ScheduleType
    {
        public ScheduleType()
        {
            Contracts = new HashSet<Contract>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
