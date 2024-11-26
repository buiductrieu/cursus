using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.UnitTests.Services
{
        [TestFixture]
        public class VoucherServiceTests
        {
            private Mock<IUnitOfWork> _mockUnitOfWork;
            private Mock<IMapper> _mockMapper;
            private Mock<IVoucherRepository> _mockVoucherRepository;
            private VoucherService _service;

            [SetUp]
            public void SetUp()
            {
                _mockUnitOfWork = new Mock<IUnitOfWork>();
                _mockMapper = new Mock<IMapper>();
                _mockVoucherRepository = new Mock<IVoucherRepository>();
                _service = new VoucherService(_mockUnitOfWork.Object, _mockMapper.Object, _mockVoucherRepository.Object);
            }

            [Test]
            public async Task CreateVoucher_ShouldCreateVoucher_WhenValidDTOProvided()
            {
                // Arrange
                var dto = new VoucherDTO { VoucherCode = "TEST", CreateDate = DateTime.MinValue, ExpireDate = DateTime.Now.AddDays(-1) };
                var entity = new Voucher();
                var mappedResult = new VoucherDTO { VoucherCode = "TEST", CreateDate = DateTime.UtcNow };

                _mockMapper.Setup(m => m.Map<Voucher>(It.IsAny<VoucherDTO>())).Returns(entity);
                _mockMapper.Setup(m => m.Map<VoucherDTO>(It.IsAny<Voucher>())).Returns(mappedResult);
                _mockVoucherRepository.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);

                // Act
                var result = await _service.CreateVoucher(dto);

                // Assert
                Assert.That(result.VoucherCode, Is.EqualTo(mappedResult.VoucherCode));
                Assert.That(result.CreateDate != DateTime.MinValue, Is.True);
                _mockVoucherRepository.Verify(r => r.AddAsync(entity), Times.Once);
                _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
            }

            [Test]
            public void DeleteVoucher_ShouldThrowException_WhenVoucherNotFound()
            {
                // Arrange
                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(It.IsAny<int>())).ReturnsAsync((Voucher)null);

                // Act & Assert
                Assert.ThrowsAsync<Exception>(() => _service.DeleteVoucher(1));
            }

            [Test]
            public async Task DeleteVoucher_ShouldDeleteVoucher_WhenVoucherFound()
            {
                // Arrange
                var entity = new Voucher { VoucherCode = "TEST" };
                var mappedResult = new VoucherDTO { VoucherCode = "TEST" };

                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(It.IsAny<int>())).ReturnsAsync(entity);
                _mockVoucherRepository.Setup(r => r.DeleteAsync(entity));
                _mockMapper.Setup(m => m.Map<VoucherDTO>(entity)).Returns(mappedResult);

                // Act
                var result = await _service.DeleteVoucher(1);

                // Assert
                Assert.That(result.VoucherCode,Is.EqualTo(mappedResult.VoucherCode));
                _mockVoucherRepository.Verify(r => r.DeleteAsync(entity), Times.Once);
                _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
            }

            [Test]
            public async Task GetVoucherByCode_ShouldReturnVoucher_WhenCodeIsValid()
            {
                // Arrange
                var entity = new Voucher { VoucherCode = "TEST" };
                var mappedResult = new VoucherDTO { VoucherCode = "TEST" };

                _mockVoucherRepository.Setup(r => r.GetByCodeAsync("TEST")).ReturnsAsync(entity);
                _mockMapper.Setup(m => m.Map<VoucherDTO>(entity)).Returns(mappedResult);

                // Act
                var result = await _service.GetVoucherByCode("TEST");

                // Assert
                Assert.That(result.VoucherCode, Is.EqualTo(mappedResult.VoucherCode));
            }

            [Test]
            public async Task GetVoucherByID_ShouldReturnVoucher_WhenIDIsValid()
            {
                // Arrange
                var entity = new Voucher { VoucherCode = "TEST" };
                var mappedResult = new VoucherDTO { VoucherCode = "TEST" };

                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(1)).ReturnsAsync(entity);
                _mockMapper.Setup(m => m.Map<VoucherDTO>(entity)).Returns(mappedResult);

                // Act
                var result = await _service.GetVoucherByID(1);

                // Assert
                Assert.That(result.VoucherCode, Is.EqualTo(mappedResult.VoucherCode));
            }

            [Test]
            public void ReceiveVoucher_ShouldThrowException_WhenVoucherNotFound()
            {
                // Arrange
                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(It.IsAny<int>())).ReturnsAsync((Voucher)null);

                // Act & Assert
                Assert.ThrowsAsync<NullReferenceException>(() => _service.ReceiveVoucher("user1", 1));
            }

            [Test]
            public async Task ReceiveVoucher_ShouldUpdateVoucher_WhenValidVoucherIDProvided()
            {
                // Arrange
                var entity = new Voucher { VoucherCode = "TEST" };

                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(1)).ReturnsAsync(entity);
                _mockVoucherRepository.Setup(r => r.UpdateAsync(entity));
                _mockUnitOfWork.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

                // Act
                var result = await _service.ReceiveVoucher("user1", 1);

                // Assert
                Assert.That(result,Is.True);
                Assert.That(entity.UserId, Is.EqualTo("user1"));
                _mockVoucherRepository.Verify(r => r.UpdateAsync(entity), Times.Once);
                _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
            }

            [Test]
            public void UpdateVoucher_ShouldThrowException_WhenVoucherNotFound()
            {
                // Arrange
                _mockUnitOfWork.Setup(u => u.VoucherRepository.GetByVourcherIdAsync(It.IsAny<int>())).ReturnsAsync((Voucher)null);

                // Act & Assert
                Assert.ThrowsAsync<Exception>(() => _service.UpdateVoucher(1, new VoucherDTO()));
            }

            [Test]
            public async Task UpdateVoucher_ShouldUpdateVoucher_WhenValidDataProvided()
            {
                // Arrange
                var entity = new Voucher();
                var updatedDTO = new VoucherDTO { VoucherCode = "UPDATED", Name = "New Name" };

                _mockUnitOfWork.Setup(u => u.VoucherRepository.GetByVourcherIdAsync(1)).ReturnsAsync(entity);
                _mockMapper.Setup(m => m.Map<VoucherDTO>(entity)).Returns(updatedDTO);

                // Act
                var result = await _service.UpdateVoucher(1, updatedDTO);

                // Assert
                Assert.That(result.VoucherCode, Is.EqualTo("UPDATED"));
                Assert.That(result.Name, Is.EqualTo("New Name"));
                _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
            }
            [Test]
            public async Task GiveVoucher_ShouldAssignVoucher_WhenValidInputProvided()
            {
                // Arrange
                string giverID = "giver123";
                string receiverID = "receiver456";
                int voucherID = 1;
                var voucher = new Voucher { Id = voucherID, IsValid = true };
                var receiver = new ApplicationUser { Id = receiverID };

                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(voucherID))
                    .ReturnsAsync(voucher);
                _mockUnitOfWork.Setup(u => u.UserRepository.ExiProfile(receiverID))
                    .Returns(Task.FromResult(receiver));
                _mockUnitOfWork.Setup(u => u.SaveChanges()).Returns(Task.CompletedTask);

                // Act
                var result = await _service.GiveVoucher(giverID, receiverID, voucherID);

                // Assert
                Assert.That(result);
                Assert.That(voucher.UserId, Is.EqualTo(receiverID));
                _mockVoucherRepository.Verify(r => r.UpdateAsync(voucher), Times.Once);
                _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
            }
            [Test]
            public void GiveVoucher_ShouldThrowException_WhenVoucherNotFound()
               {
                // Arrange
                string giverID = "giver123";
                string receiverID = "receiver456";
                int voucherID = 1;

                _mockVoucherRepository.Setup(r => r.GetByVourcherIdAsync(voucherID))
                    .ReturnsAsync((Voucher)null);

                // Act & Assert
                Assert.ThrowsAsync<InvalidOperationException>(() => _service.GiveVoucher(giverID, receiverID, voucherID));
              }
        

    }
    }

