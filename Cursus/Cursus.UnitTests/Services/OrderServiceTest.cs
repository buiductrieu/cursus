using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Presentation;
namespace Cursus.UnitTests.Repositories;
[TestFixture]
public class OrderServiceTests
{
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Mock<IEmailService> _emailService;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private Mock<IStatisticsNotificationService> _notificationService;
    private Mock<IPaymentService> _paymentService;
    private OrderService _orderService;

    [SetUp]
    public void Setup()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _emailService = new Mock<IEmailService>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _notificationService = new Mock<IStatisticsNotificationService>();
        _paymentService = new Mock<IPaymentService>();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }));
        _httpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);

        _orderService = new OrderService(
            _unitOfWork.Object,
            _mapper.Object,
            _emailService.Object,
            _notificationService.Object,
            _paymentService.Object,
            _httpContextAccessor.Object
        );
    }

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

        //[Test]
        //public async Task GetOrderHistoryAsync_ShouldReturnOrderDTOs_WhenOrdersExist()
        //{
        //    // Arrange
        //    string userId = "user1";
        //    var orders = new List<Order>
        //    {
        //        new Order { OrderId = 1, CartId = 1 },
        //        new Order { OrderId = 2, CartId = 2 }
        //    };

        //    var orderDTOs = new List<OrderDTO>
        //    {
        //        new OrderDTO { OrderId = 1 },
        //        new OrderDTO { OrderId = 2 }
        //    };
        //    _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistory(userId))
        //                   .ReturnsAsync(orders);
        //    _mapperMock.Setup(m => m.Map<List<OrderDTO>>(orders)).Returns(orderDTOs);

        //    // Act
        //    var result = await _orderService.GetOrderHistoryAsync(userId);

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count, Is.EqualTo(orderDTOs.Count));
        //    _unitOfWorkMock.Verify(u => u.OrderRepository.GetOrderHistory(userId), Times.Once);
        //    _mapperMock.Verify(m => m.Map<List<OrderDTO>>(orders), Times.Once);
        //}

        [Test]
        public void GetOrderHistoryAsync_ShouldThrowKeyNotFoundException_WhenOrdersDoNotExist()
        {
            // Arrange
            string userId = "user1";
            _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistory(userId))
                           .ReturnsAsync(new List<Order>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderHistoryAsync());
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
        _unitOfWork.Setup(u => u.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
            .ReturnsAsync((Cart)null);

        var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _orderService.CreateOrderAsync("test-user-id", null));
        //Assert.AreEqual("Cart is empty.", ex.Message);
    }

    [Test]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenCartIsValid()
    {
        var cart = new Cart
        {
            CartId = 1,
            UserId = "test-user-id",
            Total = 100,
            CartItems = new List<CartItems>
            {
                new CartItems { Course = new Course { Name = "Test Course", Price = 100 } }
            }
        };
        var order = new Order
        {
            CartId = 1,
            OrderId = 1,
            Status = OrderStatus.PendingPayment

        };

        _unitOfWork.Setup(u => u.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course"))
            .ReturnsAsync(cart);

        _paymentService.Setup(p => p.CreateTransaction("test-user-id", "PayPal", It.IsAny<string>()))
            .ReturnsAsync(new Transaction { TransactionId = 1 });
        _unitOfWork.Setup(u => u.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), null));

        _mapper.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>())).Returns(new OrderDTO { Amount = 110 });

        var result = await _orderService.CreateOrderAsync("test-user-id", null);

       // //Assert.IsNotNull(result);
       // //Assert.AreEqual(110, result.Amount);
        _unitOfWork.Verify(u => u.OrderRepository.AddAsync(It.IsAny<Order>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Test]
    public async Task GetOrderHistoryAsync_ShouldReturnOrders_WhenOrdersExist()
    {
        var orders = new List<Order> { new Order { OrderId = 1 } };
        _unitOfWork.Setup(u => u.OrderRepository.GetOrderHistory("test-user-id"))
            .ReturnsAsync(orders);

        _mapper.Setup(m => m.Map<List<OrderDTO>>(orders))
            .Returns(new List<OrderDTO> { new OrderDTO { OrderId = 1 } });

        var result = await _orderService.GetOrderHistoryAsync();

        //Assert.IsNotNull(result);
        //Assert.AreEqual(1, result.Count);
    }

    [Test]
    public void GetOrderHistoryAsync_ShouldThrow_WhenNoOrdersFound()
    {
        _unitOfWork.Setup(u => u.OrderRepository.GetOrderHistory("test-user-id"))
            .ReturnsAsync((List<Order>)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.GetOrderHistoryAsync());
        //Assert.AreEqual("Order not found.", ex.Message);
    }

    [Test]
    public async Task UpdateUserCourseAccessAsync_ShouldUpdateCourseAccess_WhenOrderIsValid()
    {
        var order = new Order
        {
            OrderId = 1,
            Cart = new Cart
            {
                CartItems = new List<CartItems> { new CartItems { Course = new Course { Name = "Test Course", Id = 1 } } }
            },
            Status = OrderStatus.Paid,
            PaidAmount = 100
        };

        var user = new ApplicationUser { Id = "I1" };
        var info = new InstructorInfo { Id = 1, UserId = "I1" };
        var wallet = new Wallet { UserId = "I1" };
        var platwallet = new PlatformWallet { Balance= 100 };

        _unitOfWork.Setup(u => u.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), "Cart,Cart.CartItems.Course"))
            .ReturnsAsync(order);

        _unitOfWork.Setup(u => u.UserRepository.ExiProfile("I1"))
            .ReturnsAsync(user);
        _unitOfWork.Setup(u => u.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null)).ReturnsAsync((CourseProgress)null);
        _unitOfWork.Setup(u => u.InstructorInfoRepository.GetAsync(It.IsAny<Expression<Func<InstructorInfo, bool>>>(), null)).ReturnsAsync(info);
        _unitOfWork.Setup(u => u.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), null)).ReturnsAsync(wallet);
        _unitOfWork.Setup(u => u.PlatformWalletRepository.GetPlatformWallet()).ReturnsAsync(platwallet);

        await _orderService.UpdateUserCourseAccessAsync(1, "I1");

        _unitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        _emailService.Verify(e => e.SendEmailSuccessfullyPurchasedCourse(user, order), Times.Once);
        _notificationService.Verify(n => n.NotifySalesAndRevenueUpdate(), Times.Once);
        _notificationService.Verify(n => n.NotifyOrderStatisticsUpdate(), Times.Once);
        Assert.That(platwallet.Balance, Is.EqualTo(130));
    }

    [Test]
    public void UpdateUserCourseAccessAsync_ShouldThrow_WhenOrderNotFound()
    {
        _unitOfWork.Setup(u => u.OrderRepository.GetAsync(It.IsAny<Expression<Func<Order, bool>>>(), "Cart,Cart.CartItems.Course"))
            .ReturnsAsync((Order)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.UpdateUserCourseAccessAsync(1, "test-user-id"));
      //  //Assert.AreEqual("Order not found or payment not completed.", ex.Message);
    }
}
