using Cursus.API.Controllers;
using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class BookmarkServiceTests
    {
        private Mock<IBookmarkService> _bookmarkServiceMock;
        private BookmarkController _controller;

        [SetUp]
        public void Setup()
        {
            _bookmarkServiceMock = new Mock<IBookmarkService>();
            _controller = new BookmarkController(_bookmarkServiceMock.Object);
        }

        [Test]
        public async Task GetBookmarks_ReturnsOkResult_WithBookmarks()
        {
            // Arrange
            var userId = "test-user-id";
            var bookmarks = new List<BookmarkDTO>
            {
                new BookmarkDTO { Id = 1, CourseName = "Course 1", Summary = "Summary 1", Price = 100, Rating = 4.5 },
                new BookmarkDTO { Id = 2, CourseName = "Course 2", Summary = "Summary 2", Price = 200, Rating = 4.0 }
            };

            _bookmarkServiceMock
                .Setup(service => service.GetFilteredAndSortedBookmarksAsync(userId, null, null, null, "asc"))
                .ReturnsAsync(bookmarks);

            // Act
            var result = await _controller.GetBookmarks(userId);

            // Assert
            var okResult = result.Result as OkObjectResult; // Use 'as' for safe casting
            Assert.IsNotNull(okResult); // Ensure okResult is not null
            var returnValue = okResult.Value as IEnumerable<BookmarkDTO>; // Safe cast
            Assert.IsNotNull(returnValue); // Ensure returnValue is not null
            Assert.AreEqual(2, ((List<BookmarkDTO>)returnValue).Count);
        }

        [Test]
        public async Task GetCourseDetails_ReturnsOkResult_WithCourseDetails()
        {
            // Arrange
            var courseId = 1;
            var courseDetail = new CourseDetailDTO
            {
                // Initialize properties here (e.g., Name, Description, etc.)
                // Example:
                // Name = "Sample Course",
                // Description = "Sample Description",
                // Price = 100,
                // Rating = 4.5
            };

            _bookmarkServiceMock
                .Setup(service => service.GetCourseDetailsAsync(courseId))
                .ReturnsAsync(courseDetail);

            // Act
            var result = await _controller.GetCourseDetails(courseId);

            // Assert
            var okResult = result.Result as OkObjectResult; // Safe cast
            Assert.IsNotNull(okResult); // Ensure okResult is not null
            var returnValue = okResult.Value as CourseDetailDTO; // Safe cast
            Assert.IsNotNull(returnValue); // Ensure returnValue is not null
        }

        [Test]
        public async Task CreateBookmark_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var bookmarkCreateDTO = new BookmarkCreateDTO
            {
                UserId = "test-user-id",
                CourseId = 1
            };

            // Act
            var result = await _controller.CreateBookmark(bookmarkCreateDTO);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult; // Safe cast
            Assert.IsNotNull(createdAtActionResult); // Ensure createdAtActionResult is not null
            Assert.AreEqual("GetBookmarks", createdAtActionResult.ActionName);
            Assert.AreEqual(bookmarkCreateDTO.UserId, createdAtActionResult.RouteValues["userId"]);
        }
    }
}
