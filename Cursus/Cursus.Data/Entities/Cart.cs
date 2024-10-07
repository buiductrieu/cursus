using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cursus.Data.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime DateCreated { get; set; }
        
        public ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
    }
}