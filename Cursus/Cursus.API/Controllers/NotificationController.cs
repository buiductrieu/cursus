using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _notificationService;
		private readonly APIResponse _response;

		public NotificationController(INotificationService notificationService, APIResponse response)
		{
			_notificationService = notificationService;
			_response = response;
		}

		/// <summary>
		/// Send notifications to the user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		[HttpPost("send")]
		public async Task<ActionResult<APIResponse>> SendNotify(string userId, string message)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
				return BadRequest("User ID and message cannot be empty.");

			await _notificationService.SendNotificationAsync(userId, message);

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = "Notification sent successfully.";

			return Ok(_response);
		}

	
	}
}
