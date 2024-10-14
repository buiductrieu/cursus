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
using NUnit.Framework;

namespace Cursus.UnitTest.Service
{
    public class CourseServiceTest
    {

        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ICourseProgressService> _progressServiceMock;
        private CourseService _courseService;
        private List<Course> _courseList;
        private List<CourseDTO> _courseDTOList;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _courseService = new CourseService(null, null, _unitOfWorkMock.Object, _mapperMock.Object);


            _courseList = new List<Course>
        {
        new Course { Id = 1, Name = "Course 1", Status = true, Category = new Category { Name = "Category 1" } },
        new Course { Id = 2, Name = "Course 2", Status = true, Category = new Category { Name = "Category 2" } },

        };
        _unitOfWorkMock.Setup(u => u.CourseRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<string>()))
                           .ReturnsAsync(_courseList.Where(c => c.Status == true).ToList());
        }
        [Test]
        public async Task GetCourseAsync_ShouldReturnFilteredCourses_WhenDataExists()
        {
            var result = await _courseService.GetCoursesAsync(null, null, null, 1, 20);


            Assert.Are; 
            Assert.AreEqual(2, result.TotalCount);   
            Assert.IsFalse(result.HasNextPage);      
            Assert.IsFalse(result.HasPreviousPage);

        }
        [Test]
      
        //[Test]
        //public async Task GetCoursesAsync_ShouldReturnFilteredCourses_WithHasNextPage_WhenSearchTermMatchesName()
        //{

        //    var result = await _courseService.GetCoursesAsync("Course 1", null, null, 1, 20);


        //    result.Items.Should().HaveCount(1);
        //    result.Items.First().Name.Should().Be("Course 1");
        //    result.HasNextPage.Should().BeFalse();
        //    result.HasPreviousPage.Should().BeFalse();
        //}
        //[Test]
        //public async Task GetCoursesAsync_ShouldReturnFilteredCourses_WhenSearchTermMatchesCategoryName()
        //{
        //    var result = await _courseService.GetCoursesAsync("Category 1", null, null, 1, 1);

        //    result.Items.Should().HaveCount(1);
        //    result.Items.First().Name.Should().Be("Category 1");
        //    result.HasNextPage.Should().BeTrue();
        //    result.HasPreviousPage.Should().BeFalse();
        //}
        //[Test]
        //public async Task GetCoursesAsync_ShouldReturnCoursesSortedAscending_WhenSortOrderIsAsc()
        //{
        //    var result = await _courseService.GetCoursesAsync(null, "name", "asc", 1, 20);

        //    result.Items.First().Name.Should().Be("Course 1");
        //    result.Items.Last().Name.Should().Be("Course 2");
        //    result.HasNextPage.Should().BeFalse();
        //    result.HasPreviousPage.Should().BeFalse();

        //}
        //[Test]
        //public async Task GetCoursesAsync_ShouldReturnCoursesSortedDescending_WhenSortOrderIsDesc()
        //{

        //    var result = await _courseService.GetCoursesAsync(null, "name", "desc", 1, 20);


        //    result.Items.First().Name.Should().Be("Course 2");
        //    result.Items.Last().Name.Should().Be("Course 1");
        //    result.HasNextPage.Should().BeFalse();  
        //    result.HasPreviousPage.Should().BeFalse(); 
        //}

        //[Test]
        //public async Task GetCoursesAsync_ShouldReturnPagedCourses_WhenPageAndPageSizeAreSpecified()
        //{          
        //    var result = await _courseService.GetCoursesAsync(null, null, null, 1, 1);  
        //    result.Items.Should().HaveCount(1);  
        //    result.PageSize.Should().Be(1);
        //    result.TotalCount.Should().Be(2);  
        //    result.HasNextPage.Should().BeTrue();  
        //    result.HasPreviousPage.Should().BeFalse();  
        //}
        //[Test]
        //public async Task GetCoursesAsync_ShouldReturnEmpty_WhenNoCoursesExist()
        //{

        //    _unitOfWorkMock.Setup(u => u.CourseRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<string>()))
        //                   .ReturnsAsync(new List<Course>());      
        //    var result = await _courseService.GetCoursesAsync(null, null, null, 1, 20);

        //    result.Items.Should().BeEmpty();
        //    result.TotalCount.Should().Be(0);
        //    result.HasNextPage.Should().BeFalse();  
        //    result.HasPreviousPage.Should().BeFalse();  
        //}

        //[Test]
        //public async Task GetRegisteredCoursesByUserIdAsync_ShouldThrowException_WhenUserDoesNotExist()
        //{
        //    var userId = "test-user";

        //    _userServiceMock.Setup(u => u.CheckUserExistsAsync(userId))
        //           .ReturnsAsync(false);

        //    var exception =  Assert.ThrowsAsync<Exception>(async () =>
        //await _courseService.GetRegisteredCoursesByUserIdAsync(userId));

        //    exception.Message.Should().Be($"User with ID {userId} not found.");
        //}


    }
}
