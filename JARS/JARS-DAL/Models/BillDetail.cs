using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class BillDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ItemName { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? BillId { get; set; }

        public virtual Bill? Bill { get; set; }
    }
}
