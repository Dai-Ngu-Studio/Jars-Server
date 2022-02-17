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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }
        [JsonIgnore]
        public virtual Category? ParentCategory { get; set; }
        [JsonIgnore]
        public virtual ICollection<Bill> Bills { get; set; }
        [JsonIgnore]
        public virtual ICollection<Category> InverseParentCategory { get; set; }
    }
}
