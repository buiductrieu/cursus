using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enums;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class PayoutRequestService : IPayoutRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CursusDbContext _db;

        public PayoutRequestService(IUnitOfWork unitOfWork, CursusDbContext db)
        {
           _unitOfWork = unitOfWork;
            _db = db;
        }

        public async Task<IEnumerable<PayoutRequestDisplayDTO>> GetApprovedPayoutRequest()
        {
            var approvedRequest = await _db.PayoutRequests
            .Include(pr => pr.Instructor)
            .ThenInclude(i => i.User)
            .Include(pr => pr.Transaction)
            .Where(pr => pr.PayoutRequestStatus == PayoutRequestStatus.Approved)
            .ToListAsync();

            return approvedRequest.Select( x => new PayoutRequestDisplayDTO
            {
                Id = x.Id,
                InstructorName = x.Instructor?.User?.UserName,
                Amount = x.Transaction?.Amount?? 0,
                CreateDate = x.CreatedDate,
                TransactionId = x.TransactionId,
                Status = x.PayoutRequestStatus
            }).ToList();
        }

        public async Task<IEnumerable<PayoutRequestDisplayDTO>> GetPendingPayoutRequest()
        {
            var pendingRequests = await _db.PayoutRequests
                .Include(pr => pr.Instructor)
                .ThenInclude(i => i.User)
                .Include(pr => pr.Transaction)
                .Where(pr => pr.PayoutRequestStatus == PayoutRequestStatus.Pending)
                .ToListAsync();

            return pendingRequests.Select(x => new PayoutRequestDisplayDTO
            {
                Id = x.Id,
                InstructorName = x.Instructor?.User?.UserName,
                Amount = x.Transaction?.Amount ?? 0,
                CreateDate = x.CreatedDate,
                TransactionId = x.TransactionId,
                Status = x.PayoutRequestStatus
            }).ToList();
        }

        public async Task<IEnumerable<PayoutRequestDisplayDTO>> GetRejectPayoutRequest()
        {
            var rejectRequest = await _db.PayoutRequests
                .Include(pr => pr.Instructor)
                .ThenInclude(i => i.User)
                .Include(pr => pr.Transaction)
                .Where(pr => pr.PayoutRequestStatus == PayoutRequestStatus.Rejected)
                .ToListAsync();
            return rejectRequest.Select(x => new PayoutRequestDisplayDTO
            {
                Id = x.Id,
                InstructorName = x.Instructor?.User?.UserName,
                Amount = x.Transaction?.Amount ?? 0,
                CreateDate = x.CreatedDate,
                TransactionId = x.TransactionId,
                Status = x.PayoutRequestStatus
            }).ToList();
        }
    }
}
