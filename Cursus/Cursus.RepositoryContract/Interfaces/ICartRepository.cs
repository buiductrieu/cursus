using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task UpdateIsPurchased(int cartId, bool isPurchased);
    }
}
