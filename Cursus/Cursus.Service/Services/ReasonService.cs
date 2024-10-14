using AutoMapper;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Data;
using Cursus.Data.DTO;

namespace Cursus.Service.Services
{
    public class ReasonService : IReasonService
    {
        private readonly IReasonRepository _reasonRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ReasonService(IReasonRepository reasonRepository, IMapper mapper, IUnitOfWork unitOfWork) 
        {
            _reasonRepository = reasonRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Reason> CreateReason (CreateReasonDTO reasonDTO)
        {
            var reason = _mapper.Map<Reason>(reasonDTO);
            if (reason.DateCancel == DateTime.MinValue)
            {
                reason.DateCancel = DateTime.UtcNow;
            }

            await _reasonRepository.AddAsync(reason);
            await _unitOfWork.SaveChanges();

            var createrReasonDTO = _mapper.Map<Reason>(reasonDTO);
            return createrReasonDTO;
        }

    }
}
