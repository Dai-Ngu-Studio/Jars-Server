using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JARS_DAL.Models
{
    public partial class AccountDevice
    {
        public string FcmToken { get; set; } = null!;
        public string? AccountId { get; set; }
        public DateTime? LastActiveDate { get; set; }

        [JsonIgnore]
        public virtual Account? Account { get; set; }
    }
}
