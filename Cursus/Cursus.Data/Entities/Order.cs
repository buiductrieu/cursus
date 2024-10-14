using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public double Amount { get; set; }
        public double PaidAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}
