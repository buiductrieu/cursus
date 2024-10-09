using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface ICartItems
    {
        public void DeleteCartItems(CartItems cartItems);
        public Task<CartItems> UpdateCartItems(CartItems cartItems);
        public Task<IEnumerable<CartItems>> GetCartItemsByCartID(int cartItemsId);
    }
}
