using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            CategoryWallets = new HashSet<CategoryWallet>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? WalletAmount { get; set; }
        public decimal? Percentage { get; set; }
        public string? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<CategoryWallet> CategoryWallets { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
