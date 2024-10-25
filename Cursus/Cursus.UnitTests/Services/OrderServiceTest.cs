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
		public async Task CreateOrderAsync_ShouldThrowBadRequestException_WhenCartIsEmpty()
		{
			string userId = "user1";
			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
						   .ReturnsAsync((Cart)null);

			var ex = Assert.ThrowsAsync<BadHttpRequestException>(() => _orderService.CreateOrderAsync(userId));
			Assert.AreEqual("Cart is empty.", ex.Message);
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
