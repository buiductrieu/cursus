using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        public DateTime DateCreated { get; set; }
        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
