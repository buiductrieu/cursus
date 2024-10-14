using Cursus.Data.DTO;
using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePayment(int orderId, string returnUrl = "https://your-default-return-url.com", string cancelUrl = "https://your-default-cancel-url.com");
        Task<TransactionDTO> CapturePayment(string token, string payerId, int orderId);
    }
}
