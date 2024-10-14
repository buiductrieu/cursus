using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly CursusDbContext _db;
        public CartRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateIsPurchased(int cartId, bool isPurchased)
        {
            var cart = await _db.Cart.FindAsync(cartId);
            if (cart != null)
            {
                cart.IsPurchased = isPurchased;
                _db.Cart.Update(cart);
               
            }
        }
    }
}
