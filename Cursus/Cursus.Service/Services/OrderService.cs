﻿using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Cursus.Service.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<OrderDTO> CreateOrderAsync(string userId)
		{
			var cart = await _unitOfWork.CartRepository.GetAsync(c => c.UserId == userId && !c.IsPurchased, "CartItems,CartItems.Course");
			if (cart == null || !cart.CartItems.Any())
				throw new BadHttpRequestException("Cart is empty.");

			var existOrder = await _unitOfWork.OrderRepository.GetAsync(c => c.CartId == cart.CartId && c.Status == OrderStatus.PendingPayment);

			if (existOrder != null)
				existOrder.Status = OrderStatus.Failed;

			double totalAmount = cart.Total;

			double taxAmount = Math.Round(totalAmount * 0.1, 2);

			var order = new Order
			{
				CartId = cart.CartId,
				Amount = totalAmount,
				PaidAmount = totalAmount + taxAmount,
				DateCreated = DateTime.Now,
				Status = OrderStatus.PendingPayment,
			};

			await _unitOfWork.OrderRepository.AddAsync(order);
			await _unitOfWork.SaveChanges();

			var OrderDTO = _mapper.Map<OrderDTO>(order);
			return OrderDTO;
		}

		public async Task UpdateUserCourseAccessAsync(int orderId, string userId)
		{
			var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId && o.Status == OrderStatus.Paid, "Cart,Cart.CartItems");

			if (order == null)
				throw new KeyNotFoundException("Order not found or payment not completed.");

			var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);
			if (user == null)
				throw new KeyNotFoundException("User not found.");

			foreach (var cartItem in order.Cart.CartItems)
			{
				var newProgress = new CourseProgress
				{
					CourseId = cartItem.CourseId,
					UserId = userId,
					Type = "Purchased",
					Date = DateTime.Now,
					IsCompleted = false
				};

				await _unitOfWork.CourseProgressRepository.AddAsync(newProgress);
			}

			await _unitOfWork.SaveChanges();
		}
	}
}