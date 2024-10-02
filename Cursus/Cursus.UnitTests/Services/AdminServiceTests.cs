using Cursus.API.Controllers;
using Cursus.Common.Helper;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class AdminServiceTests
    {

        private Mock<IAdminService> _adminServiceMock;
        private Mock<IRepository<ApplicationUser>> _userRepositoryMock;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _controller = new AdminController(_adminServiceMock.Object);
            _userRepositoryMock = new Mock<IRepository<ApplicationUser>>();
        }

        [Test]
        public async Task ToggleUserStatus_ReturnsOkResult_WhenServiceReturnsTrue()
        {
            // Arrange
            string userId = "someUserId";
            _adminServiceMock
                .Setup(s => s.ToggleUserStatusAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ToggleUserStatus(userId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.IsTrue(apiResponse.IsSuccess);
            Assert.That(apiResponse.Result, Is.EqualTo("User status has been updated"));
        }

        [Test]
        public async Task ToggleUserStatus_ReturnsBadRequest_WhenServiceReturnsFalse()
        {
            // Arrange
            string userId = "someUserId";
            _adminServiceMock
                .Setup(s => s.ToggleUserStatusAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ToggleUserStatus(userId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.IsFalse(apiResponse.IsSuccess);
            Assert.That(apiResponse.ErrorMessages, Does.Contain("Failed to update user status"));
        }

        [Test]
        public async Task ToggleUserStatus_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            string userId = "someUserId";
            _adminServiceMock
                .Setup(s => s.ToggleUserStatusAsync(userId))
                .ThrowsAsync(new Exception("Service exception"));

            // Act
            var result = await _controller.ToggleUserStatus(userId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
            Assert.IsFalse(apiResponse.IsSuccess);
            Assert.That(apiResponse.ErrorMessages, Does.Contain("Service exception"));
        }
    }
}
