using Cursus.API.Hubs;
using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace Cursus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableRateLimiting("default")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly APIResponse _response;
        

        public OrderController(IOrderService orderService, APIResponse response)
		{
			_orderService = orderService;
			_response = response;		
		}

		/// <summary>
		/// Create Order
		/// </summary>
		/// <param name="userId"></param>
		[HttpPost("create")]
		public async Task<ActionResult<APIResponse>> CreateOrder(string userId)
		{
			var order = await _orderService.CreateOrderAsync(userId);

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = order;

			return Ok(_response);
		}

		/// <summary>
		/// Confirm purchase after payment
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("confirm-purchase")]
		public async Task<ActionResult<APIResponse>> ConfirmPurchase(string userId, int orderId)
		{
			await _orderService.UpdateUserCourseAccessAsync(orderId, userId);

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = "Course access granted.";
            // Gửi thông điệp cho tất cả các client kết nối tới Hub
           
            return Ok(_response);
		}
		/// <summary>
		/// View order history
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
        [HttpGet]
        [Route("view-orderHistory")]
        public async Task<ActionResult<APIResponse>> ViewOrderHistory(string userId)
        {
            var order = await _orderService.GetOrderHistoryAsync(userId);
            if (order == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Order not found");
                return BadRequest(_response);
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = order;

            return Ok(_response);
        }
    }
}
