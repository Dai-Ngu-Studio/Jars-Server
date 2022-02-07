using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public int? WalletId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? CategoryId { get; set; }
        public int? NoteId { get; set; }
        public int? BillId { get; set; }
        public decimal? Amount { get; set; }

        public virtual Bill? Bill { get; set; }
        public virtual Note? Note { get; set; }
        public virtual Wallet? Wallet { get; set; }
    }
}
