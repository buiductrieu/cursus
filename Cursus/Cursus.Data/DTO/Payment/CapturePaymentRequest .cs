using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.DTO.Payment
{
    public class CapturePaymentRequest
    {
        [Required(ErrorMessage = "Token is required.")]
        [StringLength(100, ErrorMessage = "Token length can't be more than 100 characters.")]
        public string Token { get; set; }

       
        public string? PayId { get; set; }
      
      

        [Required(ErrorMessage = "OrderId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderId must be a positive integer.")]
        public int OrderId { get; set; }
    }
}
