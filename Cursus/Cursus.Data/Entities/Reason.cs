using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Entities
{
    public class Reason
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public DateTime DateCancel { get; set; } = DateTime.Now;

        // Điều hướng đến Course
        public Course Course { get; set; }
    }
}
