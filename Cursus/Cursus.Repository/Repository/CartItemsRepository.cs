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
    public class CartItemsRepository : Repository<CartItems>, ICartItems
    {
        private readonly CursusDbContext _db;
        public CartItemsRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }
        public async void DeleteCartItems(CartItems cartItems)
        {
            await DeleteAsync(cartItems);
        }

        public async Task<IEnumerable<CartItems>> GetCartItemsByCartID(int cartId)
        {
            return await GetAllAsync(filter: b => b.CartId == cartId);
        }

        public async Task<CartItems> UpdateCartItems(CartItems cartItems)
        {
            return await UpdateAsync(cartItems);
        }
    }
}
