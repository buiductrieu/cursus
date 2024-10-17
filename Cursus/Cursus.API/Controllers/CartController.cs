using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
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
		private readonly IUnitOfWork _unitOfWork;
		private readonly APIResponse _response;
		public CartController(ICartService cartService, APIResponse response, IUnitOfWork unitOfWork)
		{
			_cartService = cartService;
			_response = response;
			_unitOfWork = unitOfWork;
		}
        /// <summary>
        /// Delete Cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCart{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> DeleteCart(int id)
		{
			if (!ModelState.IsValid)
			{
				_response.IsSuccess = false;
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.Result = ModelState;
				return BadRequest(_response);
			}
			var result = await _cartService.DeleteCart(id);
			if (result == true)
			{
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.Created;
				_response.Result = "Instructor registered successfully";
				return Ok(result);
			}

			_response.IsSuccess = false;
			_response.StatusCode = HttpStatusCode.BadRequest;
			return BadRequest(_response);
		}
        /// <summary>
        /// Get All Carts
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCart")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> GetAllCarts()
		{
			var result = await _cartService.GetAllCart();
			if (result != null)
			{
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.Result = result;
				return Ok(_response);
			}
			_response.IsSuccess = false;
			_response.StatusCode = HttpStatusCode.BadRequest;
			return BadRequest(_response);
		}
        /// <summary>
        /// Get Cart By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("GetCart{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> GetCartById(int id)
		{
			var result = await _cartService.GetCartByID(id);
			if (result == null)
			{
				return NotFound("Your cart is empty.");
			}

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = result;
			return Ok(_response);
		}

		/// <summary>
		/// Add Course to Cart
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

		/// <summary>
		/// Get Cart by user ID
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet("my-cart")]
		public async Task<ActionResult<APIResponse>> GetCart(string userId)
		{
			var cart = await _cartService.GetCartByUserIdAsync(userId);
			if (cart == null || cart.CartItems.Count == 0)
				return NotFound("Your cart is empty.");

			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = cart;

			return Ok(_response);
		}

	}
}

