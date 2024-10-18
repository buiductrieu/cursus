using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Entities
{
    public class TransactionHistory
    {
        public int Id { get; set; }


        [ForeignKey("Wallet")]
        public int WalletId { get; set; }

        public Wallet? Wallet { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }

        public Transaction? Transaction { get; set; }

        public string Description { get; set; } = "Buy Course";

    }
}
