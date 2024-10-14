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
        [HttpPost("add-to-cart")]
        public async Task<ActionResult<APIResponse>> AddCourseToCart(string userId, int courseId)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User must be logged in.");

            await _cartService.AddCourseToCartAsync(courseId, userId);

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "Course added to cart successfully.";

            return Ok(_response);
        }
        [HttpGet("my-cart")]
        public async Task<ActionResult<APIResponse>> GetCart(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User must be logged in.");

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null || cart.CartItems.Count == 0)
            {
                return NotFound("Your cart is empty.");
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = cart;

            return Ok(_response);
        }

    }
}

