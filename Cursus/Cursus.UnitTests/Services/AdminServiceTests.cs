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
        public async Task Admin_Comments_returnsOkResult_WhenServiceReturnsTrue()
        {
            //Arrange 
            string userId = "1";
            string comment = "test comment";

            _adminServiceMock
                .Setup(s => s.AdminComments(userId, comment))
                .ReturnsAsync(true);

            //Act
            var result = await _controller.AdminComments(userId, comment) as ObjectResult;
            //Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.IsTrue(apiResponse.IsSuccess);
            Assert.That(apiResponse.Result, Is.EqualTo("Comment is sucessful"));
        }
        [Test]
        public async Task Admin_Comments_returnsBadRequestResult_WhenServiceReturnsFalse()
        {
            //Arrange 
            string userId = "123";
            string comment = "test comment";
            //Act
            _adminServiceMock
                .Setup(s => s.AdminComments(userId, comment))
                .ReturnsAsync(false);
            var result = await _controller.AdminComments(userId, comment) as ObjectResult;
            //Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.IsFalse(apiResponse.IsSuccess);
            Assert.That(apiResponse.ErrorMessages, Does.Contain("Failed to add comment"));
        }
        [Test]
        public async Task GetInstructors_returnsOkResult_WhenServiceReturnsTrue()
        {
            //Arrange 
            int userId = 1;
            var mockInstructorInfo = new Dictionary<string, object>
                {
                    { "UserName", "Test Instructor" },
                    { "Email", "instructor@example.com" },
                    { "PhoneNumber", "1234567890" },
                    { "TotalCourses", 5 },
                    { "TotalActiveCourses", 3 },
                    { "TotalEarning", 1500.0 },
                    { "TotalPayout", 1200.0 },
                    { "AverageRating", 4.5 },
                    { "AdminComment", "Great performance" }
                };
            //Act
            _adminServiceMock
                .Setup(s => s.GetInformationInstructor(userId))
                .ReturnsAsync(mockInstructorInfo);
            var result = await _controller.GetInformationInstructor(1) as ObjectResult;
            //Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.IsTrue(apiResponse.IsSuccess);
            Assert.That(apiResponse.Result, Is.Not.Null);

        }
        [Test]
        public async Task GetInstructors_returnsNotFoundResult_WhenServiceReturnsFalse()
        {
            // Arrange
            int userId = 99;

            _adminServiceMock
                .Setup(s => s.GetInformationInstructor(userId))
                .ReturnsAsync((Dictionary<string, object>?)null); 
            // Act
            var result = await _controller.GetInformationInstructor(userId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var apiResponse = result.Value as APIResponse;
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.IsSuccess);
            Assert.That(apiResponse.ErrorMessages, Does.Contain("Instructor not found"));
        }


    }
}
