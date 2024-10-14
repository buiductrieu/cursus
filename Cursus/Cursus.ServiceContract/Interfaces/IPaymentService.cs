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
        Task<string> CreatePayment(int orderId);
        Task<TransactionDTO> CapturePayment(string token, string payerId, int orderId);
    }
}
