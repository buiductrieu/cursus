using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cursus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableRateLimiting("default")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		/// <summary>
		/// Create Order
		/// </summary>
		/// <param name="userId"></param>
		[HttpPost("create")]
		public async Task<IActionResult> CreateOrder(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User must be logged in.");

			var order = await _orderService.CreateOrderAsync(userId);
			return Ok(order);
		}
	}
}
