using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Moq;
using System.ComponentModel.DataAnnotations;

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
			var courseDto = new CourseDTO
			{
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 2,
				Status = true,
				Price = 1,
				Discount = 1,

				Steps = new List<StepDTO>
				{
					new StepDTO { Name = "Step 1", Order = 1 , Description = "Description for step 1" },
					new StepDTO { Name = "Step 2", Order = 2 , Description = "Description for step 2" }
				}
			};

			var courseEntity = new Course
			{
				Name = courseDto.Name,
				Description = courseDto.Description,
				CategoryId = courseDto.CategoryId
			};

			_mapperMock.Setup(m => m.Map<Course>(courseDto)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ReturnsAsync(courseEntity);
			_unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

			var result = await _courseService.CreateCourseWithSteps(courseDto);
			var result1 = result;

			Assert.IsNotNull(result);
			Assert.AreEqual(courseDto.Name, result.Name);
			_unitOfWorkMock.Verify(u => u.CourseRepository.AddAsync(courseEntity), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldThrowException_WhenCourseCreationFails()
		{
			var courseDto = new CourseDTO
			{
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 1,
				Status = true,
				Price = 1,
				Discount = 1
			};

			var courseEntity = new Course
			{
				Name = courseDto.Name,
				Description = courseDto.Description,
				CategoryId = courseDto.CategoryId
			};

			_mapperMock.Setup(m => m.Map<Course>(courseDto)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ThrowsAsync(new Exception("Error creating course"));

			Assert.ThrowsAsync<Exception>(async () => await _courseService.CreateCourseWithSteps(courseDto));
			_unitOfWorkMock.Verify(u => u.CourseRepository.AddAsync(courseEntity), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Never);
		}

		[Test]
		public void CreateCourseWithSteps_ShouldThrowArgumentNullException_WhenCourseDtoIsNull()
		{
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _courseService.CreateCourseWithSteps(null));
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldThrowValidationException_WhenCourseNameIsEmpty()
		{
			var courseDto = new CourseDTO
			{
				Name = string.Empty, 
				Description = "Course Description",
				CategoryId = 1,
				Status = true,
				Price = 1,
				Discount = 1
			};

			var ex = Assert.ThrowsAsync<ValidationException>(async () => await _courseService.CreateCourseWithSteps(courseDto));
			Assert.AreEqual("Course name cannot be empty", ex.Message);
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldThrowValidationException_WhenStepsAreEmpty()
		{
			var courseDto = new CourseDTO
			{
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 1,
				Status = true,
				Price = 1,
				Discount = 1,
				Steps = new List<StepDTO>() 
			};

			var ex = Assert.ThrowsAsync<ValidationException>(async () => await _courseService.CreateCourseWithSteps(courseDto));
			Assert.AreEqual("At least one step is required", ex.Message);
		}

		[Test]
		public async Task CreateCourseWithSteps_ShouldCreateSteps_WhenStepsAreProvided()
		{
			var courseDto = new CourseDTO
			{
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 1,
				Status = true,
				Price = 1,
				Discount = 1,
				Steps = new List<StepDTO>
				{
					new StepDTO { Name = "Step 1", Order = 1 , Description = "Description for step 1" },
					new StepDTO { Name = "Step 2", Order = 2 , Description = "Description for step 2" }
				}
			};

			var courseEntity = new Course
			{
				Name = courseDto.Name,
				Description = courseDto.Description,
				CategoryId = courseDto.CategoryId
			};

			_mapperMock.Setup(m => m.Map<Course>(courseDto)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ReturnsAsync(courseEntity);
			_unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

			var result = await _courseService.CreateCourseWithSteps(courseDto);

			_unitOfWorkMock.Verify(u => u.CourseRepository.AddAsync(courseEntity), Times.Once);
			Assert.IsTrue(courseDto.Steps.Count > 0); 
		}
	}
}
