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
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CurrentCategoryLevel { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
