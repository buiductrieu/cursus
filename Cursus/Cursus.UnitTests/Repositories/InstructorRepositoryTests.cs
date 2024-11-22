
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Cursus.Repository.Repository;
using Cursus.Data.Models;
using Cursus.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cursus.UnitTests.Repositories
{
    [TestFixture]
    public class InstructorRepositoryTests
    {
        private InstructorRepository _repository;
        private Mock<CursusDbContext> _mockDbContext;

        [SetUp]
        public void Setup()
        {
            _mockDbContext = new Mock<CursusDbContext>();
            _repository = new InstructorRepository(_mockDbContext.Object);
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
