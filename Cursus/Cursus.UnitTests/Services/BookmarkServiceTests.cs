using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Moq;
using System.Linq.Expressions;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class BookmarkServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private BookmarkService _bookmarkService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _bookmarkService = new BookmarkService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetFilteredAndSortedBookmarksAsync_ShouldReturnBookmarks_WhenCalledWithValidParams()
        {
            // Arrange
            var userId = "user123";
            var bookmarkEntities = new List<Bookmark>
            {
                new Bookmark { Id = 1, CourseId = 1, UserId = userId },
                new Bookmark { Id = 2, CourseId = 2, UserId = userId }
            };
            var bookmarkDTOs = new List<BookmarkDTO>
            {
                new BookmarkDTO { Id = 1, CourseName = "Course 1" },
                new BookmarkDTO { Id = 2, CourseName = "Course 2" }
            };

            _unitOfWorkMock.Setup(u => u.BookmarkRepository.GetFilteredAndSortedBookmarksAsync(
                userId, null, null))
                .ReturnsAsync(bookmarkEntities);
            _mapperMock.Setup(m => m.Map<IEnumerable<BookmarkDTO>>(bookmarkEntities))
                .Returns(bookmarkDTOs);

            // Act
            var result = await _bookmarkService.GetFilteredAndSortedBookmarksAsync(userId, null, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

    }
}
