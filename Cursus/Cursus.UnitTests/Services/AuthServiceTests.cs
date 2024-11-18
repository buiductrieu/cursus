using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
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
        private Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _configurationMock = new Mock<IConfiguration>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            _emailServiceMock = new Mock<IEmailService>();

            _authService = new AuthService(
                _userManagerMock.Object,
                _configurationMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _roleManagerMock.Object,
                _emailServiceMock.Object
            );
        }

        [Test]
        public async Task LoginAsync_WithUnconfirmedEmail_ReturnsNull()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "user@example.com",
                Password = "password123"
            };
            var user = new ApplicationUser { Email = loginRequest.Username, EmailConfirmed = false };
            _userManagerMock.Setup(x => x.FindByEmailAsync(loginRequest.Username)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(true);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ForgetPassword_WithBannedUser_ThrowsException()
        {
            // Arrange
            var email = "banneduser@example.com";
            var user = new ApplicationUser { Email = email, Status = false };
            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.ForgetPassword(email));
            Assert.That(ex.Message, Is.EqualTo("User is banned and cannot reset the password."));
        }

        [Test]
        public async Task RefreshTokenAsync_WithInvalidToken_ThrowsException()
        {
            // Arrange
            var token = "invalid_refresh_token";
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetRefreshTokenAsync(token))
                .ReturnsAsync((RefreshToken)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.RefreshTokenAsync(token));
            Assert.That(ex.Message, Is.EqualTo("Invalid or expried refresh token"));
        }

        [Test]
        public async Task RefreshTokenAsync_WithInactiveToken_ThrowsException()
        {
            // Arrange
            var token = "inactive_refresh_token";
            var refreshToken = new RefreshToken { Token = token, Revoked = DateTime.UtcNow };
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetRefreshTokenAsync(token))
                .ReturnsAsync(refreshToken);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.RefreshTokenAsync(token));
            Assert.That(ex.Message, Is.EqualTo("Invalid or expried refresh token"));
        }

        [Test]
        public async Task ResetPasswordAsync_WithInvalidToken_ThrowsException()
        {
            // Arrange
            var email = "user@example.com";
            var token = "invalid_token";
            var newPassword = "NewPassword@123";
            var user = new ApplicationUser { Email = email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, newPassword))
                .ReturnsAsync(IdentityResult.Failed());

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _authService.ResetPasswordAsync(email, token, newPassword));
            Assert.That(ex.Message, Is.EqualTo("Reset password failed. Token may be invalid or expired."));
        }

        [Test]
        public async Task LogoutAsync_WithInactiveToken_ThrowsException()
        {
            // Arrange
            var refreshToken = "inactive_refresh_token";
            var token = new RefreshToken { Token = refreshToken, Revoked = DateTime.UtcNow };
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetRefreshTokenAsync(refreshToken))
                .ReturnsAsync(token);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.LogoutAsync(refreshToken));
            Assert.That(ex.Message, Is.EqualTo("Invalid or expired refresh token"));
        }

        [Test]
        public async Task LogoutAsync_WithValidToken_Succeeds()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var token = new RefreshToken { Token = refreshToken, Revoked = null, Created = DateTime.UtcNow, Expries = DateTime.UtcNow.AddMonths(12)};
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetRefreshTokenAsync(refreshToken))
                .ReturnsAsync(token);
            // Act
            await _authService.LogoutAsync(refreshToken);

            // Assert
            _unitOfWorkMock.Verify(x => x.RefreshTokenRepository.UpdateAsync(token), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
            Assert.That(token.Revoked, Is.Not.Null);
        }
    }

}
