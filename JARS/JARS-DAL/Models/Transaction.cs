using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace JARS_DAL.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            Notes = new HashSet<Note>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? WalletId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? NoteId { get; set; }
        public int? BillId { get; set; }
        public decimal? Amount { get; set; }
        [JsonIgnore]
        public virtual Bill? Bill { get; set; }
        [JsonIgnore]
        public virtual Note? Note { get; set; }
        [JsonIgnore]
        public virtual Wallet? Wallet { get; set; }
        [JsonIgnore]
        public virtual ICollection<Note> Notes { get; set; }
    }
}
