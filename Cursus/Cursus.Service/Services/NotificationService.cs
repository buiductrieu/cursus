using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;

namespace Cursus.Service.Services
{
	public class NotificationService : INotificationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public NotificationService(IUnitOfWork unitOfWork, IMapper autoMapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = autoMapper;
		}

		public async Task SendNotificationAsync(string userId, string message)
		{

			var userExists = await _unitOfWork.UserRepository.GetAsync(c => c.Id == userId);
			if (userExists == null)
				throw new KeyNotFoundException("User not found.");

			var notification = new Notification
			{
				UserId = userId,
				Message = message
			};

			await _unitOfWork.NotificationRepository.AddAsync(notification);
			await _unitOfWork.SaveChanges();
		}

		public async Task<IEnumerable<NotificationDTO>> FetchNotificationsAsync(string userId)
		{
			var userExists = await _unitOfWork.UserRepository.GetAsync(c => c.Id == userId);
			if (userExists == null)
				throw new KeyNotFoundException("User not found.");

			var notifications = await _unitOfWork.NotificationRepository.GetAllAsync(n => n.UserId == userId);

			var notifiDTO = _mapper.Map<IEnumerable<NotificationDTO>>(notifications.OrderByDescending(n => n.DateCreated));

			foreach (var notification in notifications)
			{
				if (notification.IsRead == false)
				{
					notification.IsRead = true;
					notification.IsNew = false;
				}
			}
			await _unitOfWork.SaveChanges();

			return notifiDTO;
		}
	}
}
