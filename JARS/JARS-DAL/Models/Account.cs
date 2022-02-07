using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class Account
    {
        public Account()
        {
            Wallets = new HashSet<Wallet>();
        }

        public string Id { get; set; } = null!;
        public int? AccountTypeId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public virtual AccountType? AccountType { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
