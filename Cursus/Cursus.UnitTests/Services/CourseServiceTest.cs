using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Common.Helper;

namespace Cursus.UnitTest.Service
{
    public class CourseServiceTest
    {

        private Mock<IUnitOfWork> _unitOfWorkMock;
        private IMapper _mapper;
        private Mock<IUserService> _userServiceMock;
        private Mock<ICourseProgressService> _progressServiceMock;
        private CourseService _courseService;
        private List<Course> _courseList;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _progressServiceMock = new Mock<ICourseProgressService>();

            _courseList = new List<Course>
        {
        new Course { Id = 1, Name = "Course 1", Status = true, Category = new Category { Name = "Category 1" } },
        new Course { Id = 2, Name = "Course 2", Status = true, Category = new Category { Name = "Category 2" } },

        };



            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _progressServiceMock = new Mock<ICourseProgressService>();



            _courseService = new CourseService(_progressServiceMock.Object, _userServiceMock.Object, _unitOfWorkMock.Object, _mapper);
            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<string>()))
                           .ReturnsAsync(_courseList.Where(c => c.Status == true).ToList());
        }
        [Test]
        public async Task GetCourseAsync_ShouldReturnFilteredCourses_WhenDataExists()
        {
            var result = await _courseService.GetCoursesAsync(null, null, null, 1, 20);


            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual(2, result.TotalCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);

        }
        [Test]
        public async Task GetCoursesAsync_ShouldReturnFilteredCourses_WithHasNextPage_WhenSearchTermMatchesName()
        {

            var result = await _courseService.GetCoursesAsync("Course 1", null, null, 1, 20);


            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(1, result.TotalCount);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsFalse(result.HasNextPage);
        }
        [Test]
        public async Task GetCoursesAsync_ShouldReturnFilteredCourses_WhenSearchTermMatchesCategoryName()
        {
            var result = await _courseService.GetCoursesAsync("Category 1", null, null, 1, 1);


            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(1, result.TotalCount);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsFalse(result.HasNextPage);
        }
        [Test]
        public async Task GetCoursesAsync_ShouldReturnCoursesSortedAscending_WhenSortOrderIsAsc()
        {

            var result = await _courseService.GetCoursesAsync(null, "name", "asc", 1, 20);


            Assert.IsNotNull(result.Items, "result.Items is null");
            Assert.IsTrue(result.Items.Any(), "result.Items does not contain any elements");


            Assert.AreEqual("Course 1", result.Items.First().Name);
            Assert.AreEqual("Course 2", result.Items.Last().Name);


            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsFalse(result.HasNextPage);
        }


        [Test]
        public async Task GetCoursesAsync_ShouldReturnCoursesSortedDescending_WhenSortOrderIsDesc()
        {

            var result = await _courseService.GetCoursesAsync(null, "category", "desc", 1, 20);


            Assert.IsNotNull(result.Items, "result.Items is null");
            Assert.IsTrue(result.Items.Any(), "result.Items does not contain any elements");

            // Kiểm tra thứ tự tăng dần: Phần tử đầu tiên là "Course 1"
            Assert.AreEqual("Course 2", result.Items.First().Name);
            Assert.AreEqual("Course 1", result.Items.Last().Name);

            // Kiểm tra phân trang
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsFalse(result.HasNextPage);
        }

        [Test]
        public async Task GetCoursesAsync_ShouldReturnPagedCourses_WhenPageAndPageSizeAreSpecified()
        {
            var result = await _courseService.GetCoursesAsync(null, null, null, 1, 1);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(1, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
        }
        [Test]
        public async Task GetCoursesAsync_ShouldReturnEmpty_WhenNoCoursesExist()
        {

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<Course>());
            var result = await _courseService.GetCoursesAsync(null, null, null, 1, 20);
            Assert.IsEmpty(result.Items);
            Assert.AreEqual(0, result.TotalCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
        }

        [Test]
        public async Task GetRegisteredCoursesByUserIdAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            var userId = "test-user";


            _userServiceMock.Setup(u => u.CheckUserExistsAsync(userId))
                            .ReturnsAsync(false);

            var exception = Assert.ThrowsAsync<Exception>(async () =>
        await _courseService.GetRegisteredCoursesByUserIdAsync(userId));

            Assert.AreEqual($"User with ID {userId} not found.", exception.Message);
        }

        [Test]
        public async Task GetRegisteredCoursesByUserIdAsync_ShouldReturnEmptyList_WhenNoRegisteredCourses()
        {
            var userId = "test-user";
            _userServiceMock.Setup(u => u.CheckUserExistsAsync(userId)).ReturnsAsync(true);
            _progressServiceMock.Setup(p => p.GetRegisteredCourseIdsAsync(userId)).ReturnsAsync(new List<int>());
            var result = await _courseService.GetRegisteredCoursesByUserIdAsync(userId);
            Assert.IsEmpty(result.Items);
            Assert.AreEqual(0, result.TotalCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
        }
        [Test]
        public async Task GetRegisteredCoursesByUserIdAsync_ShouldReturnListCourse_WhenCoursesExist()
        {
            var userId = "test-user";
            var CourseListId = new List<int> { 1, 2 };
            _userServiceMock.Setup(u => u.CheckUserExistsAsync(userId)).ReturnsAsync(true);
            _progressServiceMock.Setup(p => p.GetRegisteredCourseIdsAsync(userId)).ReturnsAsync(CourseListId);
            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<string>()))
                   .ReturnsAsync(_courseList);
            var result = await _courseService.GetRegisteredCoursesByUserIdAsync(userId);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual("Course 1", result.Items.First().Name);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
        }

        [Test]
        public async Task GetRegisteredCoursesByUserIdAsync_ShouldReturnPaginatedCourses_WhenCoursesExceedPageSize()
        {
            var userId = "test-user";
            var CourseListId = new List<int> { 1, 2, 3, 4 };
            _userServiceMock.Setup(u => u.CheckUserExistsAsync(userId)).ReturnsAsync(true);
            _progressServiceMock.Setup(p => p.GetRegisteredCourseIdsAsync(userId)).ReturnsAsync(CourseListId);
            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<string>()))
                  .ReturnsAsync(_courseList);
            var result = await _courseService.GetRegisteredCoursesByUserIdAsync(userId, 1, 1);


            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual("Course 1", result.Items.First().Name);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
        }
    }
}
