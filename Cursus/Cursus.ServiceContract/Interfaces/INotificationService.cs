using Cursus.Data.DTO;

namespace Cursus.ServiceContract.Interfaces
{
	public interface INotificationService
	{
		Task SendNotificationAsync(string userId, string message);
	}
}
