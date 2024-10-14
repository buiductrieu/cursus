using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    internal class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly CursusDbContext _db;
        public OrderRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }
       

            public async Task<Order> GetOrderWithCartAndItemsAsync(int orderId)
            {
                return await _db.Order
                    .Include(o => o.Cart)
                    .ThenInclude(c => c.CartItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
        

    }
}
