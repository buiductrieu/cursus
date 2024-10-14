using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.DTO.Payment
{
    public class CreatePaymentRequest
    {

       

       
        [Required(ErrorMessage = "ReturnUrl is required.")]
        [Url(ErrorMessage = "Invalid URL format for ReturnUrl.")]
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "CancelUrl is required.")]
        [Url(ErrorMessage = "Invalid URL format for CancelUrl.")]
        public string CancelUrl { get; set; }


        [Required(ErrorMessage = "OrderId is required.")]
        
        public int OrderId { get; set; }
        
       
    }
}
