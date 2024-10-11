using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;

namespace Cursus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableRateLimiting("default")]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly APIResponse _response;

		public CartController(ICartService cartService, APIResponse response)
		{
			_cartService = cartService;
			_response = response;
		}

		/// <summary>
		/// Add course to Cart
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		[HttpPost("add-to-cart")]
		public async Task<ActionResult<APIResponse>> AddCourseToCart(string userId, int courseId)
		{
			await _cartService.AddCourseToCartAsync(courseId, userId);

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = "Course added to cart successfully.";
			return Ok(_response);
		}
	}
}
