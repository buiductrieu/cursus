using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface ICart
    {
        public void DeleteCart(Cart cart);
        public Task<Cart> UpdateCart(Cart cart);
        public Task<IEnumerable<Cart>> GetCart();
        public Task<Cart> GetCartByID(int cartId);
    }
}
