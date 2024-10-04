using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Cursus.Test.Service
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

			_courseService = new CourseService(
				_repository.Object,
				_courseProgressServiceMock.Object,
				_userServiceMock.Object,
				_unitOfWorkMock.Object,
				_mapperMock.Object);
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldReturnCourseDTO_WhenCourseIsCreatedSuccessfully()
		{
			// Arrange
			var courseDto = new CourseDTO
			{
				Id = 10,
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 3,
				StartedDate = DateTime.UtcNow,
				Status = true,
				Price = 10,
				Discount = 10,
				DateCreated = DateTime.UtcNow,
				Steps = new List<StepDTO>
		{
			new StepDTO { Id = 1, CourseId = 10, Name = "Step 1", Order = 1, Description = "Description for step 1", DateCreated = DateTime.Now },
			new StepDTO { Id = 2, CourseId = 10, Name = "Step 2", Order = 2, Description = "Description for step 2", DateCreated = DateTime.Now }
		}
			};

			var courseEntity = new Course
			{
				Id = courseDto.Id,
				Name = courseDto.Name,
				Description = courseDto.Description,
				CategoryId = courseDto.CategoryId,
				DateCreated = courseDto.DateCreated,
				Status = courseDto.Status,
				Price = courseDto.Price,
				Discount = courseDto.Discount,
				StartedDate = courseDto.StartedDate,
				Steps = new List<Step>
		{
			new Step { Id = 1, CourseId = 10, Name = "Step 1", Order = 1, Description = "Description for step 1", DateCreated = DateTime.Now },
			new Step { Id = 2, CourseId = 10, Name = "Step 2", Order = 2, Description = "Description for step 2", DateCreated = DateTime.Now }
		}
			};

			_mapperMock.Setup(m => m.Map<Course>(courseDto)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ReturnsAsync(courseEntity);
			_unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);
			_unitOfWorkMock.Setup(u => u.CourseRepository.GetAllIncludeStepsAsync(It.IsAny<int>())).ReturnsAsync(courseEntity);
			_mapperMock.Setup(m => m.Map<CourseDTO>(courseEntity)).Returns(courseDto);

			var result = await _courseService.CreateCourseWithSteps(courseDto);

			Assert.IsNotNull(result);
			Assert.AreEqual(courseDto.Name, result.Name);
			Assert.AreEqual(2, result.Steps.Count);
		}

		[Test]
		public void CreateCourseWithSteps_ShouldThrowArgumentNullException_WhenCourseDtoIsNull()
		{
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _courseService.CreateCourseWithSteps(null));
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldThrowException_WhenCourseCreationFails()
		{
			var courseDto = new CourseDTO
			{
				Id = 10,
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 3,
				StartedDate = DateTime.UtcNow,
				Status = true,
				Price = 10,
				Discount = 10,
				DateCreated = DateTime.UtcNow
			};

			var courseEntity = new Course
			{
				Name = courseDto.Name,
				Description = courseDto.Description,
				CategoryId = courseDto.CategoryId,
				DateCreated = courseDto.DateCreated,
				Status = courseDto.Status,
				Price = courseDto.Price,
				Discount = courseDto.Discount,
				StartedDate = courseDto.StartedDate
			};

			_mapperMock.Setup(m => m.Map<Course>(courseDto)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ThrowsAsync(new Exception("Error creating course"));

			Assert.ThrowsAsync<Exception>(async () => await _courseService.CreateCourseWithSteps(courseDto));
			_unitOfWorkMock.Verify(u => u.CourseRepository.AddAsync(courseEntity), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Never);
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldThrowValidationException_WhenCourseNameIsEmpty()
		{
			var courseDto = new CourseDTO
			{
				Id = 10,
				Name = string.Empty,
				Description = "Course Description",
				CategoryId = 2,
				StartedDate = DateTime.UtcNow,
				Status = true,
				Price = 10,
				Discount = 10,
				DateCreated = DateTime.UtcNow,
				Steps = new List<StepDTO>
{
	new StepDTO { Id = 1, CourseId = 1, Name = "Step 1", Order = 1 , Description = "Description for step 1", DateCreated = DateTime.Now},
	new StepDTO { Id = 2, CourseId = 1, Name = "Step 2", Order = 2 , Description = "Description for step 2", DateCreated = DateTime.Now }
}
			};

			var ex = Assert.ThrowsAsync<ValidationException>(async () => await _courseService.CreateCourseWithSteps(courseDto));
			Assert.AreEqual("Course name cannot be empty", ex.Message);
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldThrowValidationException_WhenStepsAreEmpty()
		{
			var courseDto = new CourseDTO
			{
				Id = 10,
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 2,
				StartedDate = DateTime.UtcNow,
				Status = true,
				Price = 10,
				Discount = 10,
				DateCreated = DateTime.UtcNow,
				Steps = new List<StepDTO>
{
	new StepDTO { Id = 1, CourseId = 1, Name = "Step 1", Order = 1 , Description = "Description for step 1", DateCreated = DateTime.Now},
	new StepDTO { Id = 2, CourseId = 1, Name = "Step 2", Order = 2 , Description = "Description for step 2", DateCreated = DateTime.Now }
}
			};

			var ex = Assert.ThrowsAsync<ValidationException>(async () => await _courseService.CreateCourseWithSteps(courseDto));
			Assert.AreEqual("At least one step is required", ex.Message);
		}

		[Test]
        public async Task UpdateCourseWithSteps_ShouldReturnUpdatedCourse_WhenUpdateIsSuccessful()
        {
            // Arrange
            var courseDTO = new CourseDTO
            {
                Id = 1,
                Name = "Updated Course",
                Description = "Updated Description",
                CategoryId = 2,
                Steps = new List<StepDTO>
        {
            new StepDTO { Name = "Step 1", Order = 1, Description = "Description for Step 1" },
            new StepDTO { Name = "Step 2", Order = 2, Description = "Description for Step 2" }
        }
            };

            var existingCourse = new Course
            {
                Id = 1,
                Name = "Old Course",
                Description = "Old Description",
                CategoryId = 2
            };

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync(existingCourse);

            _unitOfWorkMock.Setup(u => u.CourseRepository.AnyAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(false);

            _mapperMock.Setup(m => m.Map(courseDTO, existingCourse));

            _unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<CourseDTO>(existingCourse)).Returns(courseDTO); 


            // Act
            var result = await _courseService.UpdateCourseWithSteps(courseDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(courseDTO.Name, result.Name);
            Assert.AreEqual(courseDTO.Description, result.Description);
            _unitOfWorkMock.Verify(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateCourseWithSteps_ShouldThrowException_WhenCourseNotFound()
        {
            // Arrange
            var courseDTO = new CourseDTO { Id = 1, Name = "Non-existent Course" };

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync((Course)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _courseService.UpdateCourseWithSteps(courseDTO));
            Assert.AreEqual("Course not found.", ex.Message);
        }

        [Test]
        public void UpdateCourseWithSteps_ShouldThrowException_WhenCourseNameIsNotUnique()
        {
            // Arrange
            var courseDTO = new CourseDTO { Id = 1, Name = "Duplicate Course" };
            var existingCourse = new Course { Id = 1, Name = "Existing Course" };

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync(existingCourse);

            _unitOfWorkMock.Setup(u => u.CourseRepository.AnyAsync(It.IsAny<Expression<Func<Course, bool>>>()))
                .ReturnsAsync(true); // Duplicate course name

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _courseService.UpdateCourseWithSteps(courseDTO));
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

            var result = await _courseService.DeleteCourse(course.Id);

            Assert.IsTrue(result); 
        }


        [Test]
        public void DeleteCourse_ShouldThrowException_WhenCourseNotFound()
        {
            var courseId = 1;

            _unitOfWorkMock.Setup(u => u.CourseRepository.GetAsync(It.IsAny<Expression<Func<Course, bool>>>(), null))
                .ReturnsAsync((Course)null);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _courseService.DeleteCourse(courseId));
            Assert.AreEqual("Course not found.", ex.Message);
        }

    }
}
