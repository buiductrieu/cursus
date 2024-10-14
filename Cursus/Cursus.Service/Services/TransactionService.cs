using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public TransactionService (IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IEnumerable<TransactionDTO>> GetListTransaction(int page = 1, int pageSize = 20)
        {
            IEnumerable<Transaction> transactions = await _unitOfWork.TransactionRepository.GetAllAsync(c => c.Status == Data.Enums.TransactionStatus.Completed);

            var paginatedTransactions = transactions
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToList();

            return _mapper.Map<IEnumerable<TransactionDTO>>(paginatedTransactions);
        }

        public async Task<IEnumerable<TransactionDTO>> GetListTransactionByUserId(string userId, int page = 1, int pageSize = 20)
        {

            var userExists = await _userService.CheckUserExistsAsync(userId);
            if (!userExists)
            {
                throw new KeyNotFoundException("User not found.");
            }
            IEnumerable<Transaction> transactions = await _unitOfWork.TransactionRepository.GetAllAsync(c => c.Status == Data.Enums.TransactionStatus.Completed && c.User.Id == userId);

            var paginatedTransactions = transactions
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToList();

            return _mapper.Map<IEnumerable<TransactionDTO>>(paginatedTransactions);
        }
    }
}
