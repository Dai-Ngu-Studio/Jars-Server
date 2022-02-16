using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JARS_DAL.Models
{
    public partial class Category
    {
        public Category()
        {
            Bills = new HashSet<Bill>();
            InverseParentCategory = new HashSet<Category>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public int? ParentCategoryId { get; set; }
        [JsonIgnore]
        public int? CurrentCategoryLevel { get; set; }

        public virtual Category? ParentCategory { get; set; }
        [JsonIgnore]
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Category> InverseParentCategory { get; set; }
    }
}
