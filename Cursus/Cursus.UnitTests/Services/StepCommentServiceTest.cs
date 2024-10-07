using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.Test.Service
{
    [TestFixture]
    public class StepCommentServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private StepCommentService _stepCommentService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);

            _stepCommentService = new StepCommentService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _userManagerMock.Object);
        }

        [Test]
        public async Task PostStepComment_ShouldReturnStepCommentDTO_WhenCommentIsPostedSuccessfully()
        {
            // Arrange
            var stepCommentCreateDto = new StepCommentCreateDTO
            {
                UserId = "userId",
                Content = "This is a comment",
                StepId = 10
            };

            var user = new ApplicationUser { Id = "userId", EmailConfirmed = true };
            var commentEntity = new StepComment
            {
                Id = 1,
                StepId = 10,
                UserId = "userId",
                Content = "This is a comment",
                DateCreated = DateTime.Now
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(stepCommentCreateDto.UserId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<StepComment>(stepCommentCreateDto)).Returns(commentEntity);
            _unitOfWorkMock.Setup(u => u.StepCommentRepository.AddAsync(commentEntity)).ReturnsAsync(commentEntity);
            _unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<StepCommentDTO>(commentEntity)).Returns(new StepCommentDTO { Content = "This is a comment" });

            // Act
            var result = await _stepCommentService.PostStepComment(stepCommentCreateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("This is a comment", result.Content);
        }

        [Test]
        public void PostStepComment_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var stepCommentCreateDto = new StepCommentCreateDTO { UserId = "userId" };

            _userManagerMock.Setup(um => um.FindByIdAsync(stepCommentCreateDto.UserId)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _stepCommentService.PostStepComment(stepCommentCreateDto));
        }

        [Test]
        public void PostStepComment_ShouldThrowUnauthorizedAccessException_WhenEmailNotConfirmed()
        {
            // Arrange
            var stepCommentCreateDto = new StepCommentCreateDTO { UserId = "userId" };
            var user = new ApplicationUser { Id = "userId", EmailConfirmed = false };

            _userManagerMock.Setup(um => um.FindByIdAsync(stepCommentCreateDto.UserId)).ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _stepCommentService.PostStepComment(stepCommentCreateDto));
        }

        [Test]
        public async Task GetStepCommentsAsync_ShouldReturnListOfStepCommentDTOs_WhenCommentsExist()
        {
            // Arrange
            var stepId = 10;
            var comments = new List<StepComment>
            {
                new StepComment { Id = 1, StepId = stepId, UserId = "userId1", Content = "Comment 1", DateCreated = DateTime.Now },
                new StepComment { Id = 2, StepId = stepId, UserId = "userId2", Content = "Comment 2", DateCreated = DateTime.Now }
            };

            _unitOfWorkMock.Setup(u => u.StepCommentRepository.GetAllAsync(It.IsAny<Func<StepComment, bool>>())).ReturnsAsync(comments);
            _mapperMock.Setup(m => m.Map<IEnumerable<StepCommentDTO>>(comments)).Returns(new List<StepCommentDTO>
            {
                new StepCommentDTO { Content = "Comment 1" },
                new StepCommentDTO { Content = "Comment 2" }
            });

            // Act
            var result = await _stepCommentService.GetStepCommentsAsync(stepId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetStepCommentsAsync_ShouldReturnEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            var stepId = 10;
            _unitOfWorkMock.Setup(u => u.StepCommentRepository.GetAllAsync(It.IsAny<Func<StepComment, bool>>())).ReturnsAsync(new List<StepComment>());

            // Act
            var result = await _stepCommentService.GetStepCommentsAsync(stepId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DeleteStepComment_ShouldReturnTrue_WhenCommentIsDeletedSuccessfully()
        {
            // Arrange
            var commentId = 1;
            var comment = new StepComment { Id = commentId };

            _unitOfWorkMock.Setup(u => u.StepCommentRepository.GetAsync(It.IsAny<Func<StepComment, bool>>())).ReturnsAsync(comment);
            _unitOfWorkMock.Setup(u => u.StepCommentRepository.DeleteAsync(comment));
            _unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _stepCommentService.DeleteStepComment(commentId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteStepComment_ShouldReturnFalse_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = 1;

            _unitOfWorkMock.Setup(u => u.StepCommentRepository.GetAsync(It.IsAny<Func<StepComment, bool>>())).ReturnsAsync((StepComment)null);

            // Act
            var result = await _stepCommentService.DeleteStepComment(commentId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteStepCommentIfAdmin_ShouldReturnTrue_WhenAdminDeletesCommentSuccessfully()
        {
            // Arrange
            var commentId = 1;
            var comment = new StepComment { Id = commentId };
            var adminId = "adminId";

            // Simulate that adminId is valid and that the comment exists
            _unitOfWorkMock.Setup(u => u.StepCommentRepository.GetAsync(It.IsAny<Func<StepComment, bool>>())).ReturnsAsync(comment);
            _unitOfWorkMock.Setup(u => u.StepCommentRepository.DeleteAsync(comment));
            _unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _stepCommentService.DeleteStepCommentIfAdmin(commentId, adminId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteStepCommentIfAdmin_ShouldThrowUnauthorizedAccessException_WhenAdminIdIsNull()
        {
            // Arrange
            var commentId = 1;

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _stepCommentService.DeleteStepCommentIfAdmin(commentId, null));
        }

        [Test]
        public async Task DeleteStepCommentIfAdmin_ShouldReturnFalse_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = 1;
            var adminId = "adminId";

            _unitOfWorkMock.Setup(u => u.StepCommentRepository.GetAsync(It.IsAny<Func<StepComment, bool>>())).ReturnsAsync((StepComment)null);

            // Act
            var result = await _stepCommentService.DeleteStepCommentIfAdmin(commentId, adminId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
