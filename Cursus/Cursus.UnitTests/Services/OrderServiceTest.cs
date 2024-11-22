using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enums;
using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Order = Cursus.Data.Entities.Order;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IPaymentService> _paymentServiceMock;
        private Mock<IStatisticsNotificationService> _no;
        private OrderService _orderService;


        [SetUp]
        public void Setup()
        {
            // Initialize all mocks first
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _no = new Mock<IStatisticsNotificationService>();
            _paymentServiceMock = new Mock<IPaymentService>();

            // Then create the service with the initialized mocks
            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _emailServiceMock.Object,
                _no.Object,
                _paymentServiceMock.Object

            );
        }

        #region CreateOrderAsync Tests

        [Test]
        public void CreateOrderAsync_ShouldThrowBadRequestException_WhenCartIsEmpty()
        {
            // Arrange
            string userId = "user1";
            string orderId = "order1";
            _unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
                           .ReturnsAsync((Cart)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(() => _orderService.CreateOrderAsync(userId, orderId));
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
            Assert.That(result, Is.Not.Null);
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
        [Test]
        public async Task CreateOrderAsync_ShouldCreateOrder_WhenValidCartExists()
        {
            // Arrange
            string userId = "user1";
            string voucherCode = "DISCOUNT10";
            var discount = new Voucher
            {
                VoucherCode = voucherCode,
                UserId = userId,
            };
            var cart = new Cart
            {
                CartId = 1,
                UserId = userId,
                IsPurchased = false,
                CartItems = new List<CartItems>
        {
            new CartItems { CartItemsId = 1, Course = new Course { Name = "Test Course" } }
        },
                Total = 100
            };

            var transaction = new Transaction { TransactionId = 25 };
            _unitOfWorkMock.Setup(x => x.VoucherRepository.GetAsync(It.IsAny<Expression<Func<Voucher, bool>>>(),null))
                           .ReturnsAsync(discount);
            _unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
                           .ReturnsAsync(cart);
            _unitOfWorkMock.Setup(x => x.VoucherRepository.GetAsync(It.IsAny<Expression<Func<Voucher, bool>>>(), null))
                                       .ReturnsAsync(discount);
            _unitOfWorkMock.Setup(x => x.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), null))
                           .ReturnsAsync((Order)null);
            var order = new Order();
            _unitOfWorkMock.Setup(x => x.OrderRepository.AddAsync(It.IsAny<Order>()))
                           .Callback<Order>(o => order = o);
            _unitOfWorkMock.Setup(x => x.SaveChanges()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>()))
                       .Returns(new OrderDTO { OrderId = 1 });

            _emailServiceMock.Setup(e => e.SendEmailSuccessfullyPurchasedCourse(It.IsAny<ApplicationUser>(), It.IsAny<Order>()));
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(p => p.CreateTransaction(userId, "PayPal", It.IsAny<string>()))
                              .ReturnsAsync(transaction);

            _orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, _emailServiceMock.Object,_no.Object , paymentServiceMock.Object);

            // Act
            var result = await _orderService.CreateOrderAsync(userId, voucherCode);

            // Assert
            Assert.That(result,Is.Not.Null);
            Assert.That(order.TransactionId,Is.EqualTo( transaction.TransactionId));
            Assert.That(order.Amount, Is.EqualTo(110)); // Total + Tax (100 + 10%)
            Assert.That(order.PaidAmount,Is.EqualTo( 110)); //Who create this function please check
            _unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
        }
        [Test]
        public async Task UpdateUserCourseAccessAsync_ShouldGrantAccessToCourse_WhenValidOrderExists()
        {
            // Arrange
            int orderId = 1;
            string userId = "user1";

            var order = new Order
            {
                OrderId = orderId,
                Status = OrderStatus.Paid,
                Cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItems>
            {
                new CartItems
                {
                    CourseId = 1,
                    Course = new Course { InstructorInfoId = 1 }
                }
            }
                }
            };
            var user = new ApplicationUser { Id = userId };
            var wallet = new PlatformWallet();

            var instructor = new InstructorInfo { Id = 1, TotalEarning = 0 };
            var instructorWallet = new Wallet { UserId = "instructor1", Balance = 0 };

            _unitOfWorkMock.Setup(x => x.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), "Cart,Cart.CartItems.Course"))
                           .ReturnsAsync(order);
            _unitOfWorkMock.Setup(x => x.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null))
                           .ReturnsAsync((CourseProgress)null);
            _unitOfWorkMock.Setup(x => x.InstructorInfoRepository.GetAsync(It.IsAny<Expression<Func<InstructorInfo, bool>>>(), null))
                           .ReturnsAsync(instructor);
            _unitOfWorkMock.Setup(x => x.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>(),null))
                           .ReturnsAsync(instructorWallet);
            _unitOfWorkMock.Setup(x=> x.PlatformWalletRepository.GetPlatformWallet()).ReturnsAsync(wallet);
            _unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            await _orderService.UpdateUserCourseAccessAsync(orderId, userId);

            // Assert
            Assert.That(0.7 * order.PaidAmount,Is.EqualTo( instructor.TotalEarning));
            Assert.That(0.7 * order.PaidAmount, Is.EqualTo(instructorWallet.Balance));
            _unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
        }
        [Test]
        public void CreateOrderAsync_ShouldHandleExistingPendingPaymentOrder()
        {
            // Arrange
            string userId = "user1";
            double totalAmount = 100;
            double taxAmount = 10;
            double discountAmount = 20;
            string voucherCode = "Test001";

            // Create course and cart setup as before
            var course = new Course
            {
                Id = 1,
                Name = "Test Course",
                Category = new Category { Id = 1, Name = "Test Category" },
                InstructorInfo = new InstructorInfo { Id = 1 }
            };

            var cartItems = new List<CartItems>
    {
        new CartItems
        {
            CartItemsId = 1,
            CartId = 1,
            Course = course
        }
    };

            var cart = new Cart
            {
                CartId = 1,
                UserId = userId,
                IsPurchased = false,
                CartItems = cartItems,
                Total = totalAmount
            };

            // Create transaction object that will be returned by PaymentService mock
            var transaction = new Transaction
            {
                TransactionId = 1,
                UserId = userId,
                PaymentMethod = "PayPal",
                DateCreated = DateTime.Now,
                Status = TransactionStatus.Pending,
                Description = $"User {userId} enrolls course(s): {course.Name}",
                Amount = totalAmount + taxAmount - discountAmount
            };

            var voucher = new Voucher
            {
                Id = 1,
                VoucherCode = voucherCode,
                IsValid = true,
                Name = "Test Voucher",
                CreateDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddMonths(1),
                Percentage = 20 // 20% discount
            };

            // Setup all necessary mocks
            _unitOfWorkMock.Setup(u => u.CartRepository.GetAsync(
                It.IsAny<Expression<Func<Cart, bool>>>(),
                "CartItems,CartItems.Course"))
                .ReturnsAsync(cart);

            _unitOfWorkMock.Setup(x => x.VoucherRepository.GetAsync(
                It.IsAny<Expression<Func<Voucher, bool>>>(),
                null))
                .ReturnsAsync(voucher);

            _unitOfWorkMock.Setup(x => x.TransactionRepository.GetNextTransactionId())
                .ReturnsAsync(1);

            // Important: Setup PaymentService mock to return the transaction
            _paymentServiceMock.Setup(x => x.CreateTransaction(
                userId,
                "PayPal",
                It.Is<string>(desc => desc.Contains($"User {userId} enrolls course(s): {course.Name}"))))
                .ReturnsAsync(transaction);

            // If you need to mock the mapper for discount calculation
            _mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>()))
                .Returns((Order order) => new OrderDTO
                {
                    CartId = order.CartId,
                    Amount = order.Amount,
                    PaidAmount = order.PaidAmount,
                    discountAmount = order.discountAmount,
                    discountCode = order.discountCode
                });

            // Act
            var result = _orderService.CreateOrderAsync(userId, voucherCode);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(transaction.TransactionId));

        }


    }
}
