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
                await _db.SaveChangesAsync();
            }
        }


        public async Task<bool> IsOrderCompleted(int orderId)
        {
            
            return await _db.Transactions
                .AnyAsync(t => t.OrderId == orderId && t.Status == TransactionStatus.Completed);
        }

       

        // Lấy tất cả giao dịch đang chờ xử lý (Pending) và đã quá hạn 10 phút
        public async Task<IEnumerable<Transaction>> GetPendingTransactions()
        {
            return await _db.Transactions
                .Where(t => t.Status == TransactionStatus.Pending && t.DateCreated <= DateTime.UtcNow.AddMinutes(-10))
                .ToListAsync();
        }

        // Lấy giao dịch đang chờ xử lý (Pending) dựa trên UserId và OrderId
        public async Task<Transaction?> GetPendingTransaction(string userId, int orderId)
        {
            return await _db.Transactions
                .Include(t => t.Order)
                .ThenInclude(o => o.Cart)
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId &&
                    t.OrderId == orderId &&
                    t.Status == TransactionStatus.Pending);
        }
    }
}
