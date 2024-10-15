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
	class CartServiceTests
	{
		private Mock<IUnitOfWork> _unitOfWorkMock;
		private Mock<IMapper> _mapperMock;
		private ICartService _cartService;

		[SetUp]
		public void Setup()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_mapperMock = new Mock<IMapper>();
			_cartService = new CartService(_unitOfWorkMock.Object, _mapperMock.Object);
		}

		[Test]
		public async Task AddCourseToCartAsync_ShouldAddCourseToCartSuccessfully()
		{
			string userId = "user1";
			int courseId = 1;
			var cart = new Cart { CartItems = new List<CartItems>() };

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(new ApplicationUser());
			_unitOfWorkMock.Setup(x => x.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null)).ReturnsAsync(new Course { Price = 100 });
			_unitOfWorkMock.Setup(x => x.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null)).ReturnsAsync((CourseProgress)null);
			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems")).ReturnsAsync(cart);
			_unitOfWorkMock.Setup(x => x.CartRepository.AddAsync(It.IsAny<Cart>())).ReturnsAsync(cart);
			_unitOfWorkMock.Setup(x => x.SaveChanges()).Returns(Task.CompletedTask);

			await _cartService.AddCourseToCartAsync(courseId, userId);

			Assert.AreEqual(1, cart.CartItems.Count);
			Assert.IsTrue(cart.CartItems.Any(ci => ci.CourseId == courseId));
			Assert.AreEqual(100, cart.Total);
		}

		[Test]
		public async Task AddCourseToCartAsync_ShouldThrowBadHttpRequestException_WhenCourseAlreadyInCart()
		{
			string userId = "user1";
			var course = new Course { Id = 1 };
			var cart = new Cart
			{
				CartItems = new List<CartItems>
				{
					new CartItems { CourseId = course.Id, Course = course }
				}
			};

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(new ApplicationUser());
			_unitOfWorkMock.Setup(x => x.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null)).ReturnsAsync(course);
			_unitOfWorkMock.Setup(x => x.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null)).ReturnsAsync((CourseProgress)null);
			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems")).ReturnsAsync(cart);

			var ex = Assert.ThrowsAsync<BadHttpRequestException>(() => _cartService.AddCourseToCartAsync(course.Id, userId));

			Assert.AreEqual("Course is already in the cart.", ex.Message);
		}

		[Test]
		public async Task AddCourseToCartAsync_ShouldThrowBadHttpRequestException_WhenCourseAlreadyPurchased()
		{
			string userId = "user1";
			int courseId = 1;

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(new ApplicationUser());
			_unitOfWorkMock.Setup(x => x.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null)).ReturnsAsync(new Course());
			_unitOfWorkMock.Setup(x => x.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null)).ReturnsAsync(new CourseProgress());

			var ex = Assert.ThrowsAsync<BadHttpRequestException>(() => _cartService.AddCourseToCartAsync(courseId, userId));

			Assert.AreEqual("You have already purchased this course!", ex.Message);
		}

		[Test]
		public async Task AddCourseToCartAsync_ShouldThrowKeyNotFoundException_WhenCourseDoesNotExist()
		{
			string userId = "user1";
			int courseId = 1;

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(new ApplicationUser());
			_unitOfWorkMock.Setup(x => x.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null)).ReturnsAsync((Course)null);

			var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.AddCourseToCartAsync(courseId, userId));

			Assert.AreEqual("Course not found.", ex.Message);
		}

		[Test]
		public async Task AddCourseToCartAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
		{
			string userId = "user1";
			int courseId = 1;

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync((ApplicationUser)null);

			var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _cartService.AddCourseToCartAsync(courseId, userId));

			Assert.AreEqual("User not found", ex.Message);
		}

		[Test]
		public async Task GetCartByUserIdAsync_ShouldReturnCart_WhenCartExists()
		{
			string userId = "user1";
			var cart = new Cart
			{
				CartItems = new List<CartItems> { new CartItems { CourseId = 1, Course = new Course { Price = 100 } } }
			};

			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(new ApplicationUser());
			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course")).ReturnsAsync(cart);
			_mapperMock.Setup(m => m.Map<CartDTO>(cart)).Returns(new CartDTO());

			var result = await _cartService.GetCartByUserIdAsync(userId);

			Assert.IsNotNull(result);
		}

		[Test]
		public async Task GetCartByUserIdAsync_ShouldReturnNull_WhenCartIsEmpty()
		{
			string userId = "user1";
			_unitOfWorkMock.Setup(x => x.UserRepository.ExiProfile(userId)).ReturnsAsync(new ApplicationUser());
			_unitOfWorkMock.Setup(x => x.CartRepository.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems,CartItems.Course")).ReturnsAsync((Cart)null);

			var result = await _cartService.GetCartByUserIdAsync(userId);

			Assert.IsNull(result);
		}
	}
}
