using Cursus.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.DTO
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; } 
        public string? UserId { get; set; }  
        public int OrderId { get; set; }  
        public DateTime DateCreated { get; set; }  
        public string PaymentMethod { get; set; }  
        public TransactionStatus Status { get; set; }
    }
}
