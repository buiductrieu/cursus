using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enums;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> DeleteCart(int id)
		{
           var ExCart = await _unitOfWork.CartRepository.GetCartByID(id);
            if (ExCart == null)
			{
                return false;
            }
            return await _unitOfWork.CartRepository.DeleteCart(ExCart);
			}

        public Task<IEnumerable<Cart>> GetAllCart()
			{
            return _unitOfWork.CartRepository.GetCart();
		}

        public Task<Cart> GetCartByID(int cartId)
		{
            return _unitOfWork.CartRepository.GetCartByID(cartId);
		}
        public async Task AddCourseToCartAsync(int courseId, string userId)
        {
            var course = await _unitOfWork.CourseRepository.GetAsync(c => c.Id == courseId);
            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.Cart.UserId == userId && o.Status == OrderStatus.Paid);
            if (order != null)
                throw new BadHttpRequestException("You have already purchased a course!");

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
