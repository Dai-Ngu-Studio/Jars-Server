using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JARS_DAL.Models
{
    public class TransactionWallet
    {   public int Id { get; set; }
        public string walletName { get; set; }
        public decimal? totalSpend { get; set; } 

        public decimal? totalAdded { get; set; }
        public decimal? amountLeft => totalSpend + totalAdded;
    }
}
