using Cursus.Data.DTO;
namespace Cursus.ServiceContract.Interfaces
{
	public interface IOrderService
	{
		public Task<OrderDTO> CreateOrderAsync(string userId);
	}
}
