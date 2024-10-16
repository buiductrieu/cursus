using Cursus.Data.Entities;
using Cursus.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
	public interface IOrderRepository : IRepository<Order>
	{
        Task<Order> GetOrderWithCartAndItemsAsync(int orderId);
        Task UpdateOrderStatus(int orderId,  OrderStatus newStatus);
	}
}
