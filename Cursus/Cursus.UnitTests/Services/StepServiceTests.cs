using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
    [TestFixture]
    public class StepServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<IStepRepository> _mockStepRepository;
        private Mock<ITrackingProgressRepository> _mockTrackingProgressRepository;
        private StepService _stepService;
        private Mock<ITrackingProgressRepository> _trackingStepRepository;
        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockStepRepository = new Mock<IStepRepository>();
            _mockTrackingProgressRepository = new Mock<ITrackingProgressRepository>();

            _stepService = new StepService(
                _mockStepRepository.Object,
                _mockMapper.Object,
                _mockUnitOfWork.Object,
                _mockTrackingProgressRepository.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _mockUnitOfWork.VerifyAll();
            _mockMapper.VerifyAll();
            _mockStepRepository.VerifyAll();
        }

        [Test]
        public async Task CreateStep_ValidInput_ShouldCreateStep()
        {
            // Arrange
            var stepCreateDTO = new StepCreateDTO { Name = "Step 1", Order = 1, Description = "Description" };
            var stepEntity = new Step { Id = 1, Name = "Step 1", Order = 1, Description = "Description", DateCreated = DateTime.UtcNow };
            var stepDTO = new StepDTO { Id = 1, Name = "Step 1", Order = 1, Description = "Description" };

            _mockMapper.Setup(m => m.Map<Step>(stepCreateDTO)).Returns(stepEntity);
            _mockStepRepository.Setup(r => r.AddAsync(stepEntity)).ReturnsAsync(stepEntity);
            _mockUnitOfWork.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<StepDTO>(stepEntity)).Returns(stepDTO);

            // Act
            var result = await _stepService.CreateStep(stepCreateDTO);

            // Assert
            //Assert.AreEqual(stepDTO, result);
            Assert.That(result, Is.EqualTo(stepDTO));
        }

        [Test]
        public void CreateStep_NullInput_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(() => _stepService.CreateStep(null));
        }

        [Test]
        public async Task GetStepByIdAsync_ValidId_ShouldReturnStep()
        {
            // Arrange
            var stepEntity = new Step { Id = 1, Name = "Step 1", Order = 1, Description = "Description" };
            var stepDTO = new StepDTO { Id = 1, Name = "Step 1", Order = 1, Description = "Description" };

            _mockUnitOfWork.Setup(u => u.StepRepository.GetByIdAsync(1)).ReturnsAsync(stepEntity);
            _mockMapper.Setup(m => m.Map<StepDTO>(stepEntity)).Returns(stepDTO);

            // Act
            var result = await _stepService.GetStepByIdAsync(1);

            // Assert
            //Assert.AreEqual(stepDTO, result);
            Assert.That(result, Is.EqualTo(stepDTO));
        }

        [Test]
        public void GetStepByIdAsync_InvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.StepRepository.GetByIdAsync(1)).ReturnsAsync((Step)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _stepService.GetStepByIdAsync(1));
        }

        [Test]
        public async Task GetStepsByCoursId_ValidCourseId_ShouldReturnSteps()
        {
            // Arrange
            var steps = new List<Step>
            {
                new Step { Id = 1, Name = "Step 1", Order = 1, Description = "Description" },
                new Step { Id = 2, Name = "Step 2", Order = 2, Description = "Description" }
            };
            var stepsDTO = new List<StepDTO>
            {
                new StepDTO { Id = 1, Name = "Step 1", Order = 1, Description = "Description" },
                new StepDTO { Id = 2, Name = "Step 2", Order = 2, Description = "Description" }
            };

            _mockUnitOfWork.Setup(u => u.StepRepository.GetStepsByCoursId(1)).ReturnsAsync(steps);
            _mockMapper.Setup(m => m.Map<IEnumerable<StepDTO>>(steps)).Returns(stepsDTO);

            // Act
            var result = await _stepService.GetStepsByCoursId(1);

            // Assert
            Assert.That(result, Is.EqualTo(stepsDTO));
        }

        [Test]
        public void GetStepsByCoursId_InvalidCourseId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.StepRepository.GetStepsByCoursId(1)).ReturnsAsync((IEnumerable<Step>)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _stepService.GetStepsByCoursId(1));
        }

        [Test]
        public void UpdateStep_InvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var stepUpdateDTO = new StepUpdateDTO { Id = 1, Name = "Updated Step", Order = 1, Description = "Updated Description" };

            _mockUnitOfWork.Setup(u => u.StepRepository.GetByIdAsync(1)).ReturnsAsync((Step)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _stepService.UpdateStep(stepUpdateDTO));
        }
        [Test]
        public async Task DeleteStep_ValidStepId_ShouldDeleteStep()
        {
            // Arrange
            var stepEntity = new Step { Id = 1, Name = "Step 1" };
            _mockUnitOfWork.Setup(u => u.StepRepository.GetAsync(It.IsAny<Expression<Func<Step, bool>>>(), null))
                           .ReturnsAsync(stepEntity);
            _mockUnitOfWork.Setup(u => u.StepRepository.DeleteAsync(stepEntity)).ReturnsAsync(stepEntity);
            _mockUnitOfWork.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _stepService.DeleteStep(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DeleteStep_InvalidStepId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.StepRepository.GetAsync(It.IsAny<Expression<Func<Step, bool>>>(), null))
                           .ReturnsAsync((Step)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _stepService.DeleteStep(1));
        }

        [Test]
        public async Task GetPercentageTrackingProgress_ValidInputs_ShouldReturnCompletionPercentage()
        {
            // Arrange
            var courseProgress = new CourseProgress { ProgressId = 100, CourseId = 10, UserId = "user1", IsCompleted = false };
            _mockUnitOfWork.Setup(u => u.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null))
                           .ReturnsAsync(courseProgress);
            _mockStepRepository.Setup(r => r.GetToTalSteps(10)).ReturnsAsync(5); // Total steps
            _mockTrackingProgressRepository.Setup(r => r.GetCompletedStepsCountByUserId("user1")).ReturnsAsync(5); // Completed steps
            _mockUnitOfWork.Setup(u => u.CourseProgressRepository.UpdateAsync(courseProgress)).ReturnsAsync(courseProgress);
            _mockUnitOfWork.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _stepService.GetPercentageTrackingProgress("user1", 100);

            // Assert
            Assert.That(result, Is.EqualTo(100));
            Assert.That(courseProgress.IsCompleted, Is.True);

            // Verify UpdateAsync and SaveChanges are called
            _mockUnitOfWork.Verify(u => u.CourseProgressRepository.UpdateAsync(courseProgress), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }


        [Test]
        public void StartStepAsync_InvalidUserId_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _stepService.StartStepAsync("", 1));
        }

        [Test]
        public void StartStepAsync_InvalidStepId_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.StepRepository.GetAsync(It.IsAny<Expression<Func<Step, bool>>>(), null))
                           .ReturnsAsync((Step)null);

            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(() => _stepService.StartStepAsync("user1", 1));
        }


        [Test]
        public void GetPercentageTrackingProgress_InvalidCourseProgress_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.CourseProgressRepository.GetAsync(It.IsAny<Expression<Func<CourseProgress, bool>>>(), null))
                           .ReturnsAsync((CourseProgress)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _stepService.GetPercentageTrackingProgress("user1", 100));
        }

        [Test]
        public async Task UpdateStep_ValidId_ShouldUpdateStep()
        {
            // Arrange
            var stepEntity = new Step { Id = 1, Name = "Step 1" };
            var updateDTO = new StepUpdateDTO { Id = 1, Name = "Updated Step", Order = 2, Description = "Updated Description" };
            var updatedStepDTO = new StepDTO { Id = 1, Name = "Updated Step", Order = 2, Description = "Updated Description" };

            _mockUnitOfWork.Setup(u => u.StepRepository.GetByIdAsync(1)).ReturnsAsync(stepEntity);
            _mockUnitOfWork.Setup(u => u.StepRepository.UpdateAsync(stepEntity)).ReturnsAsync(stepEntity);
            _mockMapper.Setup(m => m.Map<StepDTO>(stepEntity)).Returns(updatedStepDTO);
            _mockUnitOfWork.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _stepService.UpdateStep(updateDTO);

            // Assert
            Assert.That(result, Is.EqualTo(updatedStepDTO));
        }

    }
}
