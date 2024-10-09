using Cursus.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public DateTime DateCreated { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string Token { get; set; } = string.Empty;
    }

}
