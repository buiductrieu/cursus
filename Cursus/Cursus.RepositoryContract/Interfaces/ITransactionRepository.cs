using Cursus.Data.Entities;
using Cursus.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {

        Task UpdateTransactionStatus(int transactionId, TransactionStatus status);

       
        Task<IEnumerable<Transaction>> GetPendingTransactions();

       
        Task<Transaction?> GetPendingTransaction(int orderId);


    }
}
