using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Entities
{
    public class ArchivedTransaction
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? TransactionId { get; set; }

        public double Amount { get; set; }

        public string? Description { get; set; }

        public DateTime DateArchive { get; set; } = DateTime.Now;
    }
}
