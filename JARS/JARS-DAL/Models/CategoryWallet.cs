using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }
        [JsonIgnore]
        public virtual CategoryWallet? ParentCategory { get; set; }
        [JsonIgnore]
        public virtual ICollection<CategoryWallet> InverseParentCategory { get; set; }
        [JsonIgnore]
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
