using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper, IVoucherRepository voucherRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _voucherRepository = voucherRepository;
        }

        public async Task<VoucherDTO> CreateVoucher(VoucherDTO voucherDTO)
        {
            var voucher = _mapper.Map<Voucher>(voucherDTO);

          
            if (voucher.CreateDate == DateTime.MinValue)
            {
                voucher.CreateDate = DateTime.UtcNow;
            }

            if (voucher.ExpireDate <= DateTime.Now)
            {
                voucher.ExpireDate = voucher.ExpireDate.AddMonths(1); 
            }
                
            await _voucherRepository.AddAsync(voucher); 

          
            await _unitOfWork.SaveChanges(); 

           
            return _mapper.Map<VoucherDTO>(voucher);
        }


        public Task<VoucherDTO> DeleteVoucher(int id)
        {
            var voucher = _voucherRepository.GetByVourcherIdAsync(id).Result;
            if (voucher == null)
            {
                throw new Exception("Voucher not found");
            }
            _voucherRepository.DeleteAsync(voucher);
            _unitOfWork.SaveChanges();
            return Task.FromResult(_mapper.Map<VoucherDTO>(voucher));
        }

        public Task<VoucherDTO> GetVoucherByCode(string code)
        {
            return Task.FromResult(_mapper.Map<VoucherDTO>(_voucherRepository.GetByCodeAsync(code).Result));
        }

        public Task<VoucherDTO> GetVoucherByID(int id)
        {
            return Task.FromResult(_mapper.Map<VoucherDTO>(_voucherRepository.GetByVourcherIdAsync(id).Result));
        }

        public async Task<bool> ReceiveVoucher(string userId, int VoucherID)
        {

            var voucher = await _voucherRepository.GetByVourcherIdAsync(VoucherID);

            
            voucher.UserId = userId;
            await _voucherRepository.UpdateAsync(voucher);
            await _unitOfWork.SaveChanges();
            return true;
        }

        public async Task<VoucherDTO> UpdateVoucher(int id, VoucherDTO voucherDTO)
        {
            // Lấy Voucher cũ từ cơ sở dữ liệu dựa trên Id
            var existingVoucher = await _unitOfWork.VoucherRepository.GetByVourcherIdAsync(id);

            if (existingVoucher == null)
            {
                throw new Exception("Voucher not found");
            }

            // Cập nhật thông tin Voucher cũ với dữ liệu từ voucherDTO
            existingVoucher.VoucherCode = voucherDTO.VoucherCode;
            existingVoucher.IsValid = voucherDTO.IsValid;
            existingVoucher.Name = voucherDTO.Name;
            existingVoucher.CreateDate = voucherDTO.CreateDate;
            existingVoucher.ExpireDate = voucherDTO.ExpireDate;
            existingVoucher.Percentage = voucherDTO.Percentage;

            // Lưu lại các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.SaveChanges();

            // Ánh xạ lại Entity thành DTO và trả về
            return _mapper.Map<VoucherDTO>(existingVoucher);
        }
    }
    
}
