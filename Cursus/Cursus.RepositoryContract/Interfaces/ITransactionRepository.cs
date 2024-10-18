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

        // Kiểm tra xem người dùng đã hoàn thành khóa học dựa trên UserId và CourseId
        Task<bool> IsOrderCompleted(int transactionId);

        // Lấy giao dịch đang chờ xử lý (Pending) dựa trên UserId và OrderId (Thay thế CourseId bằng OrderId)       
        Task<IEnumerable<Transaction>> GetPendingTransactions();
       
        Task<Transaction?> GetPendingTransaction(int transactionId);


    }
}
