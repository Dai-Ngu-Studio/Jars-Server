using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            CategoryWallets = new HashSet<CategoryWallet>();
            Transactions = new HashSet<Transaction>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
