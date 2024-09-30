using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.UnitTest.Services
{
    public class AuthServiceTests
    {
        private AuthService _authService;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IConfiguration> _configurationMock;

        [SetUp]
        public void Setup()
        {
            // Khởi tạo Mock cho UserManager<ApplicationUser>
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Tạo mock cho các section của IConfiguration
            var jwtKeySection = new Mock<IConfigurationSection>();
            jwtKeySection.Setup(x => x.Value).Returns("q7X5vH!tP3@zM4wKj9bQ*F6sY#eL8uR1"); // Giá trị "Key" từ file cấu hình

            var jwtIssuerSection = new Mock<IConfigurationSection>();
            jwtIssuerSection.Setup(x => x.Value).Returns("http://localhost:5159"); // Giá trị "Issuer" từ file cấu hình

            var jwtAudienceSection = new Mock<IConfigurationSection>();
            jwtAudienceSection.Setup(x => x.Value).Returns("http://localhost:5159"); // Giá trị "Audience" từ file cấu hình

            // Khởi tạo Mock cho IConfiguration và thiết lập trả về các section tương ứng
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x.GetSection("JWT:Key")).Returns(jwtKeySection.Object);
            _configurationMock.Setup(x => x.GetSection("JWT:Issuer")).Returns(jwtIssuerSection.Object);
            _configurationMock.Setup(x => x.GetSection("JWT:Audience")).Returns(jwtAudienceSection.Object);

            // Khởi tạo AuthService với UserManager và IConfiguration mock
            _authService = new AuthService(_userManagerMock.Object, _configurationMock.Object);

        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "test@example.com",
                Password = "Password123!"
            };

            var user = new ApplicationUser
            {
                Email = loginRequest.Username,
                Id = "userId123"
            };

            // Giả lập việc tìm thấy user với email đã cung cấp
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Username)).ReturnsAsync(user);

            // Giả lập việc kiểm tra mật khẩu thành công
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(true);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Token);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task LoginAsync_UserNotFound_ReturnsUserNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "notfound@example.com",
                Password = "Password123!"
            };

            // Giả lập không tìm thấy user với email đã cung cấp
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Username)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Token);
            Assert.AreEqual("User not found", result.ErrorMessage);
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsInvalidPassword()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new ApplicationUser
            {
                Email = loginRequest.Username,
                Id = "userId123"
            };

            // Giả lập việc tìm thấy user với email đã cung cấp
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Username)).ReturnsAsync(user);

            // Giả lập việc kiểm tra mật khẩu thất bại
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(false);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Token);
            Assert.AreEqual("Invalid Password", result.ErrorMessage);
        }

        [Test]
        public async Task LoginAsync_EmptyUsername_ReturnsFailure()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "",
                Password = "Password123!"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Token);
            Assert.AreEqual("User not found", result.ErrorMessage);
        }

        [Test]
        public async Task LoginAsync_EmptyPassword_ReturnsFailure()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "test@example.com",
                Password = ""
            };

            var user = new ApplicationUser
            {
                Email = loginRequest.Username,
                Id = "userId123"
            };

            // Giả lập việc tìm thấy user với email đã cung cấp
            _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Username)).ReturnsAsync(user);

            // Giả lập việc kiểm tra mật khẩu thất bại (do mật khẩu rỗng)
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(false);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Token);
            Assert.AreEqual("Invalid Password", result.ErrorMessage);
        }
    }
}
