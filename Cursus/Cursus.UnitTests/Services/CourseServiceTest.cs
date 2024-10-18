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
		public async Task CreateCourseWithSteps_ShouldReturnCourseDTO_WhenCourseIsCreatedSuccessfully()
		{
			// Arrange
			var courseCreateDTO = new CourseCreateDTO
			{
				Name = "New Course",
				Description = "Course Description",
				CategoryId = 1,
				Status = true,
				Price = 10,
				Discount = 10,
				Steps = new List<StepCreateDTO>
				{
					new StepCreateDTO { CourseId = 1, Name = "Step 1", Order = 1, Description = "Description for step 1" },
					new StepCreateDTO { CourseId = 1, Name = "Step 2", Order = 2, Description = "Description for step 2" }
				}
			};

			var category = new Category
			{
				Id = courseCreateDTO.CategoryId,
				Name = "Existing Category"
			};

			var courseEntity = new Course
			{
				Id = 1,
				Name = courseCreateDTO.Name,
				Description = courseCreateDTO.Description,
				CategoryId = courseCreateDTO.CategoryId,
				DateCreated = DateTime.Now,
				DateModified = DateTime.Now,
				Status = courseCreateDTO.Status,
				Price = courseCreateDTO.Price,
				Discount = courseCreateDTO.Discount,
				StartedDate = DateTime.Now,
				Steps = new List<Step>
				{
					new Step { Id = 1, CourseId = 1, Name = "Step 1", Order = 1, Description = "Description for step 1", DateCreated = DateTime.Now },
					new Step { Id = 2, CourseId = 1, Name = "Step 2", Order = 2, Description = "Description for step 2", DateCreated = DateTime.Now }
				}
			};

			_mapperMock.Setup(m => m.Map<CourseDTO>(It.IsAny<Course>())).Returns((Course course) => new CourseDTO
			{
				Id = course.Id,
				Name = course.Name,
				Description = course.Description,
				CategoryId = course.CategoryId,
				DateCreated = course.DateCreated,
				DateModified = course.DateModified,
				Status = course.Status,
				Price = course.Price,
				Discount = course.Discount,
				StartedDate = course.StartedDate,
				Rating = course.Rating,
				Steps = course.Steps.Select(step => new StepDTO
				{
					Id = step.Id,
					CourseId = step.CourseId,
					Name = step.Name,
					Order = step.Order,
					Description = step.Description,
					DateCreated = step.DateCreated
				}).ToList()
			});

			_mapperMock.Setup(m => m.Map<Course>(courseCreateDTO)).Returns(courseEntity);
			_unitOfWorkMock.Setup(u => u.CategoryRepository.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null))
				.ReturnsAsync(category);
			_unitOfWorkMock.Setup(u => u.CourseRepository.AddAsync(courseEntity)).ReturnsAsync(courseEntity);
			_unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);
			_unitOfWorkMock.Setup(u => u.CourseRepository.GetAllIncludeStepsAsync(It.IsAny<int>())).ReturnsAsync(courseEntity);
			_mapperMock.Setup(m => m.Map<CourseCreateDTO>(courseEntity)).Returns(courseCreateDTO);

			// Act
			var result = await _courseService.CreateCourseWithSteps(courseCreateDTO);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(courseCreateDTO.Name, result.Name);
			Assert.AreEqual(2, result.Steps.Count);
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

		

	}

}
