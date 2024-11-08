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

		public NotificationService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
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

	}
}
