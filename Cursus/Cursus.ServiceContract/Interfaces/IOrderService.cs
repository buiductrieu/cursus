using Cursus.Data.DTO;
namespace Cursus.ServiceContract.Interfaces
{
	public interface IOrderService
	{
		Task<OrderDTO> CreateOrderAsync(string userId);
		Task UpdateUserCourseAccessAsync(int orderId, string userId);
        Task<List<OrderDTO>> GetOrderHistoryAsync(string userId);

    }
}
