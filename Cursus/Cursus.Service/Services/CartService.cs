using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Cursus.Service.Services
{
	public class CartService : ICartService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CartService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task AddCourseToCartAsync(int courseId, string userId)
		{
			var course = await _unitOfWork.CourseRepository.GetAsync(c => c.Id == courseId);
			if (course == null)
				throw new KeyNotFoundException("Course not found.");

			var cart = await _unitOfWork.CartRepository.GetAsync(c => c.UserId == userId && !c.IsPurchased, "CartItems");

			if (cart == null)
			{
				cart = new Cart
				{
					UserId = userId,
					Total = 0,
					IsPurchased = false,
					CartItems = new List<CartItems>()
				};

				await _unitOfWork.CartRepository.AddAsync(cart);
				await _unitOfWork.SaveChanges();
			}

			var cartItemExists = cart.CartItems.Any(ci => ci.CourseId == courseId);
			if (cartItemExists)
				throw new BadHttpRequestException("Course is already in the cart.");

			var cartItem = new CartItems
			{
				CourseId = courseId,
				CartId = cart.CartId,
				Price = course.Price
			};

			cart.CartItems.Add(cartItem);
			cart.Total = cart.CartItems.Sum(ci => ci.Price);

			await _unitOfWork.SaveChanges();
		}

		public async Task<CartDTO> GetCartByUserIdAsync(string userId)
		{
			var cart = await _unitOfWork.CartRepository.GetAsync(c => c.UserId == userId && !c.IsPurchased, "CartItems,CartItems.Course");

			if (cart == null)
				return null;

			var cartDTO = _mapper.Map<CartDTO>(cart);

			return cartDTO;
		}

	}
}
