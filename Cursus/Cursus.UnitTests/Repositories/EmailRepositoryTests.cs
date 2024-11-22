
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Cursus.Repository.Repository;
using Cursus.Data.Models;
using Cursus.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Cursus.Common.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cursus.UnitTests.Repositories
{
    [TestFixture]
    public class EmailRepositoryTests
    {
        private EmailRepository _repository;
        private Mock<CursusDbContext> _mockDbContext;
        private Mock<ILogger<EmailRepository>> _mockLogger;
        private Mock<IOptions<EmailSetting>> _mockEmailSetting;

        [SetUp]
        public void Setup()
        {
            _mockDbContext = new Mock<CursusDbContext>();
            _mockLogger = new Mock<ILogger<EmailRepository>>();
            _mockEmailSetting = new Mock<IOptions<EmailSetting>>();
            _repository = new EmailRepository(_mockEmailSetting.Object, _mockLogger.Object);
        }

        [Test]
        public async Task TestMethod1_ShouldPerformExpectedAction()
        {
            // Arrange
            
            // Act
            
            // Assert
            
        }
        
        // Additional tests to cover 100% of the methods and cases
    }
}
