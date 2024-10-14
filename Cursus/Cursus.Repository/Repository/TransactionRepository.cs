using Cursus.Data.Entities;
using Cursus.Data.Enums;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly CursusDbContext _db;

        public TransactionRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }

        // Cập nhật trạng thái giao dịch
        public async Task UpdateTransactionStatus(int transactionId, TransactionStatus status)
        {
            var transaction = await _db.Transactions.FindAsync(transactionId);
            if (transaction != null)
            {
                transaction.Status = status;
               
            }
        }

       
        public async Task<IEnumerable<Transaction>> GetPendingTransactions()
        {
            return await _db.Transactions
                .Where(t => t.Status == TransactionStatus.Pending)
                .ToListAsync();
        }


        public async Task<Transaction?> GetPendingTransaction(int orderId)
        {
            return await _db.Transactions
                .FirstOrDefaultAsync(t => t.OrderId == orderId && t.Status == TransactionStatus.Pending);
        }
    }
}
