using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;

namespace Cursus.UnitTests.Services
{
	[TestFixture]
	class OrderServiceTests
	{
		private Mock<IUnitOfWork> _unitOfWorkMock;
		private Mock<IMapper> _mapperMock;
		private Mock<IEmailService> _emailServiceMock;
		private IOrderService _orderService;

		[SetUp]
		public void Setup()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_mapperMock = new Mock<IMapper>();
			_emailServiceMock = new Mock<IEmailService>();
			_orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, _emailServiceMock.Object);
		}

		[Test]
		public async Task CreateOrderAsync_ShouldCreateOrderSuccessfully()
		{
			string userId = "user1";
			var cart = new Cart
			{
				CartId = 1,
				UserId = userId,
				CartItems = new List<CartItems>
				{
					new CartItems { CourseId = 1, Course = new Course { Price = 100 } }
				},
				Total = 100
			};

			var order = new Order
			{
				OrderId = 1,
				CartId = cart.CartId,
				Amount = cart.Total,
				PaidAmount = cart.Total + (cart.Total * 0.1),
				Status = OrderStatus.PendingPayment
			};

			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
						   .ReturnsAsync(cart);

			_unitOfWorkMock.Setup(x => x.OrderRepository.AddAsync(It.IsAny<Order>()))
						   .ReturnsAsync(order);

			_mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>()))
					   .Returns(new OrderDTO { OrderId = order.OrderId });

			_unitOfWorkMock.Setup(x => x.SaveChanges()).Returns(Task.CompletedTask);

			var result = await _orderService.CreateOrderAsync(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(order.OrderId, result.OrderId);
		}

		[Test]
		public async Task CreateOrderAsync_ShouldThrowBadRequestException_WhenCartIsEmpty()
		{
			string userId = "user1";
			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
						   .ReturnsAsync((Cart)null);

			var ex = Assert.ThrowsAsync<BadHttpRequestException>(() => _orderService.CreateOrderAsync(userId));
			Assert.AreEqual("Cart is empty.", ex.Message);
		}

		[Test]
		public async Task UpdateUserCourseAccessAsync_ShouldGrantCourseAccessSuccessfully()
		{
			int orderId = 1;
			string userId = "user1";
			var order = new Order
			{
				OrderId = orderId,
				Status = OrderStatus.Paid,
				Cart = new Cart
				{
					CartItems = new List<CartItems>
					{
						new CartItems { CourseId = 1, Course = new Course() }
					}
				}
			};
			var user = new ApplicationUser { Id = userId };

			_unitOfWorkMock.Setup(x => x.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), "Cart,Cart.CartItems.Course"))
						   .ReturnsAsync(order);

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId))
						   .ReturnsAsync(user);

			_unitOfWorkMock.Setup(x => x.CourseProgressRepository.AddAsync(It.IsAny<CourseProgress>()))
						   .ReturnsAsync(new CourseProgress());

			_unitOfWorkMock.Setup(x => x.SaveChanges()).Returns(Task.CompletedTask);

			await _orderService.UpdateUserCourseAccessAsync(orderId, userId);

			_emailServiceMock.Verify(x => x.SendEmailSuccessfullyPurchasedCourse(user, order), Times.Once);
		}

		[Test]
		public async Task UpdateUserCourseAccessAsync_ShouldThrowException_WhenOrderNotFoundOrNotPaid()
		{
			int orderId = 1;
			string userId = "user1";
			_unitOfWorkMock.Setup(x => x.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), "Cart,Cart.CartItems.Course"))
						   .ReturnsAsync((Order)null);

			var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.UpdateUserCourseAccessAsync(orderId, userId));
			Assert.AreEqual("Order not found or payment not completed.", ex.Message);
		}
	}
}
