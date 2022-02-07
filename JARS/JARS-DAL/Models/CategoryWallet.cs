using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class CategoryWallet
    {
        public int Id { get; set; }
        public int? WalletId { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }

        public virtual Wallet? Wallet { get; set; }
    }
}
