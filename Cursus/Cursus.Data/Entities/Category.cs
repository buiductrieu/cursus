using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public DateTime DateCreated { get; set; }
        
        public bool Status { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
