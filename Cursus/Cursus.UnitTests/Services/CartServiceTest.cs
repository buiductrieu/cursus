using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private CartService _cartService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cartService = new CartService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWorkMock = null;
            _mapperMock = null;
            _cartService = null;
        }

        #region Happy Path Scenarios

        [Test]
        public async Task GetAllCart_ReturnsAllCarts()
        {
            // Arrange
            var carts = new List<Cart> { new Cart { CartId = 1 }, new Cart { CartId = 2 } };
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCart()).ReturnsAsync(carts);

            // Act
            var result = await _cartService.GetAllCart();

            // Assert
            //Assert.AreEqual(2, result.Count());
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetCartByID_ReturnsCart_WhenCartExists()
        {
            // Arrange
            var cart = new Cart { CartId = 1 };
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(1)).ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartByID(1);

            // Assert
            // Assert.AreEqual(1, result.CartId);
            Assert.That(result.CartId, Is.EqualTo(1));
        }

        #endregion

        #region Edge Cases

        [Test]
        public async Task GetCartByID_ReturnsNull_WhenCartDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(It.IsAny<int>())).ReturnsAsync((Cart)null);

            // Act
            var result = await _cartService.GetCartByID(1);

            // Assert
            Assert.That(result,Is.Null);
        }

        #endregion

        #region Error Conditions

        [Test]
        public void GetAllCart_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCart()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _cartService.GetAllCart());
        }

        [Test]
        public void GetCartByID_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _cartService.GetCartByID(1));
        }

        #endregion

        #region Boundary Values

        [Test]
        public async Task GetCartByID_ReturnsCart_WhenCartIdIsMinValue()
        {
            // Arrange
            var cart = new Cart { CartId = int.MinValue };
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(int.MinValue)).ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartByID(int.MinValue);

            // Assert
            Assert.That(result.CartId, Is.EqualTo(int.MinValue));
        }

        [Test]
        public async Task GetCartByID_ReturnsCart_WhenCartIdIsMaxValue()
        {
            // Arrange
            var cart = new Cart { CartId = int.MaxValue };
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(int.MaxValue)).ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartByID(int.MaxValue);

            // Assert
            Assert.That(result.CartId, Is.EqualTo(int.MaxValue));
        }

        #endregion

        #region Null/Empty Inputs

        [Test]
        public void AddCourseToCartAsync_ThrowsNullReferenceException_WhenUserIdIsNull()
        {
            // Arrange
            var courseId = 1;

            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _cartService.AddCourseToCartAsync(courseId, null));
        }

        [Test]
        public void AddCourseToCartAsync_ThrowsNullReferenceException_WhenUserIdIsEmpty()
        {
            // Arrange
            var courseId = 1;

            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _cartService.AddCourseToCartAsync(courseId, string.Empty));
        }

        #endregion

        #region Invalid Inputs

        [Test]
        public void AddCourseToCartAsync_ThrowsNullReferenceException_WhenCourseIdIsNegative()
        {
            // Arrange
            var userId = "user1";
            var courseId = -1;

            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _cartService.AddCourseToCartAsync(courseId, userId));
        }

        #endregion

        #region State Verification

        [Test]
        public async Task DeleteCart_DeletesCart_WhenCartExists()
        {
            // Arrange
            var cart = new Cart { CartId = 1 };
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(1)).ReturnsAsync(cart);
            _unitOfWorkMock.Setup(u => u.CartRepository.DeleteCart(cart)).ReturnsAsync(true);

            // Act
            var result = await _cartService.DeleteCart(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteCart_ReturnsFalse_WhenCartDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(It.IsAny<int>())).ReturnsAsync((Cart)null);

            // Act
            var result = await _cartService.DeleteCart(1);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Exception Handling

        [Test]
        public void DeleteCart_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.CartRepository.GetCartByID(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _cartService.DeleteCart(1));
        }

        #endregion
    }
}
