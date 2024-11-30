using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Cursus.Service.Hubs
{
	public class NotificationHub : Hub
	{
		private readonly INotificationService _notificationService;

		public NotificationHub(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		public async Task SendNotificationToUser(string userId, string message)
		{
			await Clients.User(userId).SendAsync("ReceiveNotification", "message");
		}

		public async Task GetUserNotifications(string userId)
		{
			var notifications = await _notificationService.FetchNotificationsAsync(userId);
			await Clients.Caller.SendAsync("LoadNotifications", notifications);
		}
	}
}