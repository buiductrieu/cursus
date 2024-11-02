using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AuthService _authService;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _configurationMock = new Mock<IConfiguration>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _authService = new AuthService(
                _userManagerMock.Object,
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _roleManagerMock.Object
            );
        }

        [Test]
        public void LoginAsync_WithInvalidCredentials_ThrowsException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "invalid@example.com",
                Password = "wrongpassword"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginRequest.Username))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.LoginAsync(loginRequest));
            Assert.That(ex.Message, Is.EqualTo("Username or password is incorrect!"));
        }

        [Test]
        public async Task RegisterAsync_WithValidData_ReturnsUser()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                UserName = "test@example.com",
                Password = "Password@123",
                Address = "123 Test Street",
                PhoneNumber = "1234567890",
                Role = "User"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.UsernameExistsAsync(userRegisterDTO.UserName)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.UserRepository.PhoneNumberExistsAsync(userRegisterDTO.PhoneNumber)).ReturnsAsync(false);

            var user = new ApplicationUser { UserName = userRegisterDTO.UserName, Email = userRegisterDTO.UserName };
            _mapperMock.Setup(x => x.Map<ApplicationUser>(userRegisterDTO)).Returns(user);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), userRegisterDTO.Password)).ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.FindByEmailAsync(userRegisterDTO.UserName)).ReturnsAsync(user);

            // Act
            var result = await _authService.RegisterAsync(userRegisterDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Email, Is.EqualTo(userRegisterDTO.UserName));
        }

        [Test]
        public async Task RegisterAsync_WithExistingUsername_ThrowsException()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                UserName = "test@example.com",
                Password = "Password@123",
                Address = "123 Test Street",
                PhoneNumber = "1234567890",
                Role = "User"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.UsernameExistsAsync(userRegisterDTO.UserName)).ReturnsAsync(true);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.RegisterAsync(userRegisterDTO));
            Assert.That(ex.Message, Is.EqualTo("Username is existed"));
        }

        [Test]
        public async Task RegisterAsync_WithInvalidRole_ThrowsException()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                UserName = "test@example.com",
                Password = "Password@123",
                Address = "123 Test Street",
                PhoneNumber = "1234567890",
                Role = "InvalidRole"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.UsernameExistsAsync(userRegisterDTO.UserName)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.UserRepository.PhoneNumberExistsAsync(userRegisterDTO.PhoneNumber)).ReturnsAsync(false);

            var user = new ApplicationUser { UserName = userRegisterDTO.UserName, Email = userRegisterDTO.UserName };
            _mapperMock.Setup(x => x.Map<ApplicationUser>(userRegisterDTO)).Returns(user);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), userRegisterDTO.Password)).ReturnsAsync(IdentityResult.Success);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.RegisterAsync(userRegisterDTO));
            Assert.That(ex.Message, Is.EqualTo("Role is not valid"));
        }

        [Test]
        public async Task ConfirmEmail_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var username = "test@example.com";
            var token = "valid_token";
            var user = new ApplicationUser { Email = username, Id = "1" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(username)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.ConfirmEmail(username, token);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ConfirmEmail_WithInvalidUser_ThrowsException()
        {
            // Arrange
            var username = "nonexistent@example.com";
            var token = "valid_token";

            _userManagerMock.Setup(x => x.FindByEmailAsync(username)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.ConfirmEmail(username, token));
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }
    }
}
