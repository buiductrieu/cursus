using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Cursus.UnitTests.Services
{
	[TestFixture]
	public class CourseServiceTests
	{
		private Mock<IUnitOfWork> _unitOfWorkMock;
		private Mock<IMapper> _mapperMock;
		private Mock<ICourseProgressService> _courseProgressServiceMock;
		private Mock<IUserService> _userServiceMock;
		private Mock<ICourseRepository> _repository;
		private ICourseService _courseService;

		[SetUp]
		public void Setup()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_mapperMock = new Mock<IMapper>();
			_courseProgressServiceMock = new Mock<ICourseProgressService>();
			_userServiceMock = new Mock<IUserService>();
			_repository = new Mock<ICourseRepository>();

			_courseService = new CourseService
				(
					_courseProgressServiceMock.Object,
					_userServiceMock.Object,
					_unitOfWorkMock.Object,
					_mapperMock.Object
				);
		}

		[Test]
		public void CreateCourseWithSteps_ShouldThrowNullReferenceException_WhenCourseDtoIsNull()
		{
			Assert.ThrowsAsync<NullReferenceException>(async () => await _courseService.CreateCourseWithSteps(null));
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldNullReferenceException_WhenCourseCreationFails()
		{
			var courseCreateDTO = new CourseCreateDTO
			{
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 3,
				Status = true,
				Price = 10,
				Discount = 10,
			};

			var courseEntity = new Course();

			_mapperMock.Setup(m => m.Map<Course>(courseCreateDTO)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ThrowsAsync(new BadHttpRequestException("Error creating course"));

			Assert.ThrowsAsync<NullReferenceException>(async () => await _courseService.CreateCourseWithSteps(courseCreateDTO));
		}

        [Test]
        public async Task UpdateCourseWithSteps_ShouldThrowException_WhenCourseNotFound()
        {
            // Arrange
            var courseUpdateDTO = new CourseUpdateDTO { Id = 1, Name = "Non-existent Course" };

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync((Course)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _courseService.UpdateCourseWithSteps(courseUpdateDTO));
            Assert.AreEqual("Course not found.", ex.Message);
        }

        [Test]
        public async Task UpdateCourseWithSteps_ShouldThrowException_WhenCourseNameIsNotUnique()
        {
            // Arrange
            var courseUpdateDTO = new CourseUpdateDTO { Id = 1, Name = "Duplicate Course" };
            var existingCourse = new Course { Id = 1, Name = "Existing Course" };

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync(existingCourse);

            _unitOfWorkMock.Setup(u => u.CourseRepository.AnyAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(true); // Duplicate course name

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _courseService.UpdateCourseWithSteps(courseUpdateDTO));
            Assert.AreEqual("Course name must be unique.", ex.Message);
        }

        [Test]
        public async Task DeleteCourse_ShouldReturnTrue_WhenCourseIsDeletedSuccessfully()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Test Course" };

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync(course);

            _unitOfWorkMock.Setup(u => u.SaveChanges())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _courseService.DeleteCourse(course.Id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteCourse_ShouldThrowException_WhenCourseNotFound()
        {
            // Arrange
            var courseId = 1;

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync((Course)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _courseService.DeleteCourse(courseId));
            Assert.AreEqual("Course not found.", ex.Message);
        }

    }


}


