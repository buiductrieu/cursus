using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task UpdateUserProfile_ExistingUserAndDifferentUsername_UpdatesUserAndReturnsDTO()
        {
            // Arrange
            var existingUser = new ApplicationUser { Id = "1", UserName = "existinguser" };
            var userProfileUpdateDTO = new UserProfileUpdateDTO { UserName = "newusername", Address = "newaddress", PhoneNumber = "newphone", Email = "newemail@example.com" };

            _mockUnitOfWork.Setup(uow => uow.UserRepository.ExiProfile("1")).ReturnsAsync(existingUser);
            _mockUnitOfWork.Setup(uow => uow.UserRepository.UsernameExistsAsync("newusername")).ReturnsAsync(false);
            _mockUnitOfWork.Setup(uow => uow.UserRepository.UpdProfile(existingUser)).ReturnsAsync(existingUser);

            // Act
            var result = await _userService.UpdateUserProfile("1", userProfileUpdateDTO);

            // Assert
            Assert.That(result.UserName, Is.EqualTo(userProfileUpdateDTO.UserName));
            Assert.That(result.Address, Is.EqualTo(userProfileUpdateDTO.Address));
            Assert.That(result.PhoneNumber, Is.EqualTo(userProfileUpdateDTO.PhoneNumber));
            Assert.That(result.Email, Is.EqualTo(userProfileUpdateDTO.Email));

            _mockUnitOfWork.Verify(uow => uow.UserRepository.ExiProfile("1"), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.UserRepository.UsernameExistsAsync("newusername"), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.UserRepository.UpdProfile(existingUser), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChanges(), Times.Once);
        }
    }
}
