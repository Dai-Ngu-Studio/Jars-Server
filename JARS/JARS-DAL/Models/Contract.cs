using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class Contract
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? AccountId { get; set; }
        public int? ScheduleTypeId { get; set; }
        public int? CategoryId { get; set; }
        public int? NoteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Amount { get; set; }
        public int? BillId { get; set; }

        public virtual Bill? Bill { get; set; }
        public virtual Note? Note { get; set; }
        public virtual ScheduleType? ScheduleType { get; set; }
    }
}
