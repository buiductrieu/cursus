using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Cursus.UnitTest.Services
{
    [TestFixture]
    public class Tests
    {
        private InstructorService _instructorService;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IInstructorInfoRepository> _instructorInfoRepositoryMock;

        [SetUp]
        public void Setup()
        {
            // Kh?i t?o Mock cho UserManager<ApplicationUser>
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Kh?i t?o Mock cho IInstructorInfoRepository
            _instructorInfoRepositoryMock = new Mock<IInstructorInfoRepository>();

            // Kh?i t?o Mock cho IUnitOfWork v� thi?t l?p tr? v? InstructorInfoRepository mock
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.InstructorInfoRepository).Returns(_instructorInfoRepositoryMock.Object);

            // Kh?i t?o InstructorService v?i c�c Mock
            _instructorService = new InstructorService(_userManagerMock.Object, _unitOfWorkMock.Object);
        }

        private bool Isvalid (object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model, null, null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Test]
        public async Task InstructorAsync_ValidRegisterInstructorDTO_ReturnsSuccess()
        {
            // Arrange: T?o d? li?u gi? l?p cho vi?c th�m Instructor
            var registerInstructorDTO = new RegisterInstructorDTO
            {
                Email = "test@example.com",
                Phone = "123456789",
                Address = "Test Address",
                Password = "Password123!",
                CardName = "Test Card",
                CardProvider = "Test Provider",
                CardNumber = "1234567890",
                SubmitCertificate = "None",
            };

            // Gi? l?p vi?c t?o user th�nh c�ng b?ng UserManager
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Gi? l?p ph??ng th?c AddAsync c?a InstructorInfoRepository tr? v? Task.CompletedTask
            _instructorInfoRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<InstructorInfo>())).Returns(Task.CompletedTask);

            // Act: G?i h�m InstructorAsync
            var result = await _instructorService.InstructorAsync(registerInstructorDTO);

            // Assert: Ki?m tra k?t qu? tr? v? c� th�nh c�ng hay kh�ng
            Assert.IsTrue(result.Succeeded);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            _instructorInfoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<InstructorInfo>()), Times.Once);
        }
        [Test]
        public void RegisterInstructorDTO_EmptyField()
        {
            //Tr??ng h?p t?o DTO r?ng
            var dto = new RegisterInstructorDTO();
            var isValid = Isvalid(dto, out var validationResults);

            Assert.IsFalse(isValid);
            Assert.AreEqual(13, validationResults.Count);
        }
        [Test]
        public void RegisterInstructorDTO_EmptyFields_ReturnsValidationErrors()
        {
            // Arrange: T?o DTO v?i c�c tr??ng r?ng
            var dto = new RegisterInstructorDTO();

            // Act: Ki?m tra t�nh h?p l? c?a DTO
            var isValid = Isvalid(dto, out var validationResults);

            // Assert: ??m b?o DTO kh�ng h?p l? v� c� l?i
            Assert.IsFalse(isValid);
            Assert.AreEqual(13, validationResults.Count); // C� 13 tr??ng Required trong DTO
        }

        [Test]
        public void RegisterInstructorDTO_InvalidEmailFormat_ReturnsValidationError()
        {
            // Arrange: T?o DTO v?i email kh�ng h?p l?
            var dto = new RegisterInstructorDTO
            {
                Email = "invalid-email", // Email kh�ng h?p l?
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Birthday = DateTime.Now,
                Phone = "1234567890",
                Address = "123 Street",
                TaxNumber = "1234567890",
                CardName = "Visa",
                CardProvider = "Visa Provider",
                CardNumber = "1234567812345678",
                SubmitCertificate = "Certificate"
            };

            // Act
            var isValid = Isvalid(dto, out var validationResults);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Exists(vr => vr.ErrorMessage == "Invalid email format"));
        }

        [Test]
        public void RegisterInstructorDTO_PasswordsDoNotMatch_ReturnsValidationError()
        {
            // Arrange: T?o DTO v?i Password v� ConfirmPassword kh�ng kh?p
            var dto = new RegisterInstructorDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword!", // Password kh�ng kh?p
                FullName = "John Doe",
                Birthday = DateTime.Now,
                Phone = "1234567890",
                Address = "123 Street",
                TaxNumber = "1234567890",
                CardName = "Visa",
                CardProvider = "Visa Provider",
                CardNumber = "1234567812345678",
                SubmitCertificate = "Certificate"
            };

            // Act
            var isValid = Isvalid(dto, out var validationResults);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Exists(vr => vr.ErrorMessage == "Passwords do not match"));
        }

        [Test]
        public void RegisterInstructorDTO_InvalidPhoneNumber_ReturnsValidationError()
        {
            // Arrange: T?o DTO v?i s? ?i?n tho?i kh�ng h?p l?
            var dto = new RegisterInstructorDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Birthday = DateTime.Now,
                Phone = "InvalidPhone", // S? ?i?n tho?i kh�ng h?p l?
                Address = "123 Street",
                TaxNumber = "1234567890",
                CardName = "Visa",
                CardProvider = "Visa Provider",
                CardNumber = "1234567812345678",
                SubmitCertificate = "Certificate"
            };

            // Act
            var isValid = Isvalid(dto, out var validationResults);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Exists(vr => vr.ErrorMessage == "Invalid phone number"));
        }

        [Test]
        public void RegisterInstructorDTO_InvalidTaxNumber_ReturnsValidationError()
        {
            // Arrange: T?o DTO v?i Tax Number kh�ng h?p l? (ph?i l� 10 ch? s?)
            var dto = new RegisterInstructorDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Birthday = DateTime.Now,
                Phone = "1234567890",
                Address = "123 Street",
                TaxNumber = "1234", // Tax Number kh�ng h?p l?
                CardName = "Visa",
                CardProvider = "Visa Provider",
                CardNumber = "1234567812345678",
                SubmitCertificate = "Certificate"
            };

            // Act
            var isValid = Isvalid(dto, out var validationResults);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Exists(vr => vr.ErrorMessage == "Tax number must be exactly 10 digits"));
        }

        [Test]
        public void RegisterInstructorDTO_ValidDTO_ReturnsSuccess()
        {
            // Arrange: T?o DTO h?p l?
            var dto = new RegisterInstructorDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FullName = "John Doe",
                Birthday = DateTime.Now,
                Phone = "1234567890",
                Address = "123 Street",
                TaxNumber = "1234567890",
                CardName = "Visa",
                CardProvider = "Visa Provider",
                CardNumber = "1234567812345678",
                SubmitCertificate = "Certificate"
            };

            // Act
            var isValid = Isvalid(dto, out var validationResults);

            // Assert
            Assert.IsTrue(isValid);
            Assert.IsEmpty(validationResults);
        }

    }
}