﻿using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User must be logged in.");

			var order = await _orderService.CreateOrderAsync(userId);

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = order;

			return Ok(_response);
		}

		/// <summary>
		/// Confirm purchase after payment
		/// </summary
		/// <param name="userId"></param>
		/// <param name="orderId"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("confirm-purchase")]
		public async Task<ActionResult<APIResponse>> ConfirmPurchase(string userId, int orderId)
		{
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User must be logged in.");

			await _orderService.UpdateUserCourseAccessAsync(orderId, userId);

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = "Course access granted.";

			return Ok(_response);
		}
	}
}