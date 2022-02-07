using System;
using System.Collections.Generic;

namespace JARS_DAL.Models
{
    public partial class Category
    {
        public Category()
        {
            Bills = new HashSet<Bill>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
