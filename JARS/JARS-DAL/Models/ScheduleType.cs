using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class ScheduleType
    {
        public ScheduleType()
        {
            Contracts = new HashSet<Contract>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
