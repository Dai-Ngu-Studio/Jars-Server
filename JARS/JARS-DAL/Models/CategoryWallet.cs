using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class CategoryWallet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? WalletId { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }

        public virtual Wallet? Wallet { get; set; }
    }
}
