using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
            Transactions = new HashSet<Transaction>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Name { get; set; }
        public decimal? Amount { get; set; }
        public decimal? LeftAmount { get; set; }
        public int? CategoryId { get; set; }
        public int? ContractId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Contract? Contract { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
