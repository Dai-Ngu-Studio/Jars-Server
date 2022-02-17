using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JARS_DAL.Models
{
    public partial class Account
    {
        public Account()
        {
            Contracts = new HashSet<Contract>();
            Wallets = new HashSet<Wallet>();
        }

        public string Id { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Contract> Contracts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
