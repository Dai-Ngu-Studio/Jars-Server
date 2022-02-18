using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JARS_DAL.Models
{
    public partial class CategoryWallet
    {
        public CategoryWallet()
        {
            InverseParentCategory = new HashSet<CategoryWallet>();
            Wallets = new HashSet<Wallet>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }

        public virtual CategoryWallet? ParentCategory { get; set; }
        public virtual ICollection<CategoryWallet> InverseParentCategory { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
