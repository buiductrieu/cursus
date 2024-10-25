using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enums;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Cursus.Service.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;
		private readonly IPaymentService _paymentService;

		public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_emailService = emailService;
		}

		public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IPaymentService paymentService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_emailService = emailService;
			_paymentService = paymentService;
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


			var transaction = await _paymentService.CreateTransaction(userId, "PayPal", $"User {userId} enrolls course(s): {string.Join(", ", cart.CartItems.Select(ci => ci.Course.Name))}");

			var order = new Order
			{
				CartId = cart.CartId,
				Amount = totalAmount,
				PaidAmount = totalAmount + taxAmount,
				DateCreated = DateTime.Now,
				Status = OrderStatus.PendingPayment,
				TransactionId = transaction.TransactionId,
				Transaction = transaction
			};


			await _unitOfWork.OrderRepository.AddAsync(order);
			await _unitOfWork.SaveChanges();

			var OrderDTO = _mapper.Map<OrderDTO>(order);
			return OrderDTO;
		}

		public async Task UpdateUserCourseAccessAsync(int orderId, string userId)
		{
			var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId && o.Cart.UserId == userId && o.Status == OrderStatus.Paid, "Cart,Cart.CartItems.Course");

			if (order == null)
				throw new KeyNotFoundException("Order not found or payment not completed.");

			var user = await _unitOfWork.UserRepository.ExiProfile(userId);
			if (user == null)
				throw new KeyNotFoundException("User not found.");

			bool flag = true;
			
			foreach (var cartItem in order.Cart.CartItems)
			{

				if (flag == true)
				{
					var existedProgress =  await _unitOfWork.CourseProgressRepository.GetAsync(c => c.CourseId == cartItem.CourseId && c.UserId == userId);
					if (existedProgress == null)
					{
						flag = false;
					}
					else
					{
						throw new BadHttpRequestException("this order has been access granted");
					}
				}

					var newProgress = new CourseProgress
					{
						CourseId = cartItem.CourseId,
						UserId = userId,
						Type = "Enrollment",
						Date = DateTime.Now,
						IsCompleted = false
					};

					await _unitOfWork.CourseProgressRepository.AddAsync(newProgress);

					cartItem.Course.InstructorInfo = await _unitOfWork.InstructorInfoRepository.GetAsync(i => i.Id == cartItem.Course.InstructorInfoId);
					(await _unitOfWork.WalletRepository.GetAsync(w => w.UserId == cartItem.Course.InstructorInfo.UserId)).Balance += order.PaidAmount * 70 / 100;

					(await _unitOfWork.PlatformWalletRepository.GetPlatformWallet()).Balance += order.PaidAmount * 30 / 100;
				
			}

			await _unitOfWork.SaveChanges();

			_emailService.SendEmailSuccessfullyPurchasedCourse(user, order);

		}
	}
}
