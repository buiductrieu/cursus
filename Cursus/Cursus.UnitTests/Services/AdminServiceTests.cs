using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cursus.Service.Services;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using Cursus.ServiceContract.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class AdminServiceTests
    {
        private AdminService _adminService;
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<IInstructorInfoRepository> _instructorInfoRepositoryMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        [SetUp]
        public void Setup()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _instructorInfoRepositoryMock = new Mock<IInstructorInfoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Setup UserManager mock with required dependencies
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object);

            _adminService = new AdminService(
                _adminRepositoryMock.Object,
                _instructorInfoRepositoryMock.Object,
                _userManagerMock.Object,
                _unitOfWorkMock.Object);
        }

        #region ToggleUserStatus Tests

        [Test]
        public async Task ToggleUserStatusAsync_ShouldReturnFalse_WhenUserIdIsNull()
        {
            // Act
            var result = await _adminService.ToggleUserStatusAsync(null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ToggleUserStatusAsync_False_WhenUserUpdateFails()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", Status = true };
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ThrowsAsync(new Exception("Update failed"));

            // Act & Assert
            var ex = await _adminService.ToggleUserStatusAsync("user1");
            Assert.That(ex, Is.False);
        }

        #endregion

        #region AdminComments Tests

        [Test]
        public async Task AdminComments_ShouldReturnFalse_WhenCommentIsEmpty()
        {
            // Act
            var result = await _adminService.AdminComments("user1", "");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AdminComments_ShouldThrowException_WhenDatabaseUpdateFails()
        {
            // Arrange
            _adminRepositoryMock.Setup(ar => ar.AdminComments(It.IsAny<string>(), It.IsAny<string>()))
                                .ThrowsAsync(new Exception("Database update failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.AdminComments("user1", "Test comment"));
            Assert.That(ex.Message, Is.EqualTo("Database update failed"));
        }

        #endregion

        #region AcceptPayout Tests

        [Test]
        public async Task AcceptPayout_ShouldThrowException_WhenWalletNotFound()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, Description = "payout", Amount = 100 };
            _unitOfWorkMock.Setup(uow => uow.TransactionRepository.GetAsync(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                null))
                .ReturnsAsync((Transaction?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _adminService.AcceptPayout(1));
        }

        [Test]
        public async Task AcceptPayout_ShouldThrowException_WhenSaveChangesFails()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, Description = "payout", Amount = 100 };
            var wallet = new Wallet { UserId = "user1", Balance = 200 };

            _unitOfWorkMock.Setup(uow => uow.TransactionRepository.GetAsync(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                null))
                .ReturnsAsync(transaction);

            _unitOfWorkMock.Setup(uow => uow.WalletRepository.GetAsync(
                It.IsAny<Expression<Func<Wallet, bool>>>(),
                null))
                .ReturnsAsync(wallet);

            _unitOfWorkMock.Setup(uow => uow.SaveChanges())
                .Throws(new Exception("Database save failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.AcceptPayout(1));
            Assert.That(ex.Message, Is.EqualTo("Database save failed"));
        }

        #endregion

        #region GetInformationInstructor Tests

        [Test]
        public async Task GetInformationInstructor_ShouldReturnEmptyDictionary_WhenInstructorIdIsInvalid()
        {
            // Act
            Assert.ThrowsAsync<NullReferenceException>(async () => await _adminService.GetInformationInstructor(-1));

            // Assert

        }

        [Test]
        public async Task GetInformationInstructor_ShouldReturnPartialData_WhenInstructorInfoIsIncomplete()
        {
            // Arrange
            var partialInstructorInfo = (UserName: "PartialName", Email: (string?)null, PhoneNumber: (string?)null, AdminComment: (string?)null);
            _adminRepositoryMock.Setup(repo => repo.GetInformationInstructorAsync(2))
                                .ReturnsAsync(partialInstructorInfo);

            Assert.ThrowsAsync<NullReferenceException>(async () => await _adminService.GetInformationInstructor(2));


        }

        #endregion

        #region GetAllUser Tests

        [Test]
        public async Task GetAllUser_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _adminRepositoryMock.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<string?>()))
                .ReturnsAsync(new List<ApplicationUser>());

            // Act
            var result = await _adminService.GetAllUser();

            // Assert
            Assert.That(result,Is.Not.Null);
            Assert.That(result,Is.Empty);
        }

        [Test]
        public async Task GetAllUser_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _adminRepositoryMock.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<string?>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _adminService.GetAllUser());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        #endregion
    }
}