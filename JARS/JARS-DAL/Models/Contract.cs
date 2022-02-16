using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JARS_DAL.Models
{
    public partial class Contract
    {
        public Contract()
        {
            Bills = new HashSet<Bill>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? AccountId { get; set; }
        public int? ScheduleTypeId { get; set; }
        public int? CategoryId { get; set; }
        public int? NoteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Amount { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Note? Note { get; set; }
        public virtual ScheduleType? ScheduleType { get; set; }
        [JsonIgnore]
        public virtual ICollection<Bill> Bills { get; set; }
    }
}
