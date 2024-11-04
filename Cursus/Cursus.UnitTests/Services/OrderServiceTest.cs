using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailService> _emailServiceMock;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, _emailServiceMock.Object);
        }

        #region CreateOrderAsync Tests

        [Test]
        public void CreateOrderAsync_ShouldThrowBadRequestException_WhenCartIsEmpty()
        {
            // Arrange
            string userId = "user1";
            _unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
                           .ReturnsAsync((Cart)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(() => _orderService.CreateOrderAsync(userId));
            Assert.That(ex.Message, Is.EqualTo("Cart is empty."));
        }


        #endregion

        #region UpdateUserCourseAccessAsync Tests

        [Test]
        public void UpdateUserCourseAccessAsync_ShouldThrowKeyNotFoundException_WhenOrderNotFoundOrNotPaid()
        {
            // Arrange
            int orderId = 1;
            string userId = "user1";
            _unitOfWorkMock.Setup(x => x.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), "Cart,Cart.CartItems.Course"))
                           .ReturnsAsync((Order)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.UpdateUserCourseAccessAsync(orderId, userId));
            Assert.That(ex.Message, Is.EqualTo("Order not found or payment not completed."));
        }

        #endregion

        #region GetOrderHistoryAsync Tests

        [Test]
        public async Task GetOrderHistoryAsync_ShouldReturnOrderDTOs_WhenOrdersExist()
        {
            // Arrange
            string userId = "user1";
            var orders = new List<Order>
            {
                new Order { OrderId = 1, CartId = 1 },
                new Order { OrderId = 2, CartId = 2 }
            };

            var orderDTOs = new List<OrderDTO>
            {
                new OrderDTO { OrderId = 1 },
                new OrderDTO { OrderId = 2 }
            };
            _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistory(userId))
                           .ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<List<OrderDTO>>(orders)).Returns(orderDTOs);

            // Act
            var result = await _orderService.GetOrderHistoryAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(orderDTOs.Count));
            _unitOfWorkMock.Verify(u => u.OrderRepository.GetOrderHistory(userId), Times.Once);
            _mapperMock.Verify(m => m.Map<List<OrderDTO>>(orders), Times.Once);
        }

        [Test]
        public void GetOrderHistoryAsync_ShouldThrowKeyNotFoundException_WhenOrdersDoNotExist()
        {
            // Arrange
            string userId = "user1";
            _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistory(userId))
                           .ReturnsAsync(new List<Order>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderHistoryAsync(userId));
            Assert.That(ex.Message, Is.EqualTo("Order not found."));
            _unitOfWorkMock.Verify(u => u.OrderRepository.GetOrderHistory(userId), Times.Once);
        }

        #endregion

    }
}
