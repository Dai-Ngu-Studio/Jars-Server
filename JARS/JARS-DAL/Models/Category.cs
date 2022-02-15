using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }

        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Category> InverseParentCategory { get; set; }
    }
}
