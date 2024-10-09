using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class CartRepository : Repository<Cart>, ICart
    {
        private readonly CursusDbContext _db;
        public CartRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }
        public async void DeleteCart(Cart cart)
        {
            cart.IsPurchased = true;
            await UpdateAsync(cart);

        }

        public async Task<IEnumerable<Cart>> GetCart()
        {
           return await GetAllAsync();
        }

        public async Task<Cart> GetCartByID(int cartId)
        {
            return await GetAsync(filter: b => b.CartId == cartId);
        }

        public async Task<Cart> UpdateCart(Cart cart)
        {
            return await UpdateAsync(cart);
        }
    }
}
