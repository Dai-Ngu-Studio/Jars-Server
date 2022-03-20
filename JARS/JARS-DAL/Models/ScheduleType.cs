using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
