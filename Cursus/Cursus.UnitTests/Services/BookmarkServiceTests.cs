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
<<<<<<< Updated upstream

        [Test]
        public async Task GetCourseDetailsAsync_ShouldReturnCourseDetails_WhenCourseExists()
        {
            var courseId = 1;
            var courseEntity = new Course
            {
                Id = courseId,
                Name = "Course 1",
                Steps = new List<Step>
        {
            new Step { Id = 1, Description = "Step 1" },
            new Step { Id = 2, Description = "Step 2" }
        }
            };
            var courseDTO = new CourseDTO
            {
                Id = courseId,
                Name = "Course 1",
                Steps = new List<StepDTO>
        {
            new StepDTO { Id = 1, Description = "Step 1" },
            new StepDTO { Id = 2, Description = "Step 2" }
        }
            };

            // Thiết lập mock cho repository
            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(c => c.Id == courseId, "Steps"))
                .ReturnsAsync(courseEntity);

            // Thiết lập mock cho mapper
            _mapperMock.Setup(m => m.Map<CourseDTO>(courseEntity))
                .Returns(courseDTO);

            // Act
            var result = await _bookmarkService.GetCourseDetailsAsync(courseId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(courseId, result.Id);
            Assert.AreEqual("Course 1", result.Name);
            Assert.AreEqual(2, result.Steps.Count); 
            Assert.AreEqual("Step 1", result.Steps[0].Description);
            Assert.AreEqual("Step 2", result.Steps[1].Description);
        }

        [Test]
        public async Task CreateBookmarkAsync_ShouldReturnCourseDTO_WhenBookmarkIsCreatedSuccessfully()
        {
            // Arrange
            var bookmarkCreateDTO = new BookmarkCreateDTO { UserId = "user123", CourseId = 1 };
            var user = new ApplicationUser { Id = "user123", UserName = "TestUser" };
            var course = new Course { Id = 1, Name = "Course 1", Price = 100, Rating = 5 };

            _unitOfWorkMock.Setup(u => u.UserRepository.ExiProfile(bookmarkCreateDTO.UserId))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null)).ReturnsAsync(course);

            _unitOfWorkMock.Setup(u => u.BookmarkRepository.GetAsync(It.IsAny<Expression<Func<Bookmark, bool>>>(), null)).ReturnsAsync((Bookmark)null); 

            _unitOfWorkMock.Setup(u => u.BookmarkRepository.AddAsync(It.IsAny<Bookmark>()))
                .ReturnsAsync(new Bookmark()); 

          
            _unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            await _bookmarkService.CreateBookmarkAsync(bookmarkCreateDTO);

            // Assert
            _unitOfWorkMock.Verify(u => u.UserRepository.ExiProfile(bookmarkCreateDTO.UserId), Times.Once);
            _unitOfWorkMock.Verify(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null), Times.Once);
            _unitOfWorkMock.Verify(u => u.BookmarkRepository.GetAsync(It.IsAny<Expression<Func<Bookmark, bool>>>(), null), Times.Once);
            _unitOfWorkMock.Verify(u => u.BookmarkRepository.AddAsync(It.IsAny<Bookmark>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

=======
>>>>>>> Stashed changes
    }
}
