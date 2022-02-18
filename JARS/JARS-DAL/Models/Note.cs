using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JARS_DAL.Models
{
    public partial class Note
    {
        public Note()
        {
            Contracts = new HashSet<Contract>();
            Transactions = new HashSet<Transaction>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? AddedDate { get; set; }
        public string? Comments { get; set; }
        public string? Image { get; set; }
        public int? TransactionId { get; set; }
        public int? ContractId { get; set; }
        [JsonIgnore]
        public virtual Contract? Contract { get; set; }
        [JsonIgnore]
        public virtual Transaction? Transaction { get; set; }

        [JsonIgnore]
        public virtual ICollection<Contract> Contracts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
