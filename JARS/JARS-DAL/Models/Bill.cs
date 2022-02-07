using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
            Contracts = new HashSet<Contract>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Name { get; set; }
        public decimal? Amount { get; set; }
        public int? RecurringTransactionId { get; set; }
        public decimal? LeftAmount { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
