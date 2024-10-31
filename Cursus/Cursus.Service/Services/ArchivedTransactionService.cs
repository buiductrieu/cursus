using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;

namespace Cursus.Service.Services
{
    public class ArchivedTransactionService : IArchivedTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ArchivedTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ArchivedTransactionDTO> ArchiveTransaction(int transactionId)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction is not found");
            }

            var archivedTransaction = _mapper.Map<ArchivedTransaction>(transaction);

            await _unitOfWork.TransactionRepository.DeleteAsync(transaction);

            await _unitOfWork.ArchivedTransactionRepository.AddAsync(archivedTransaction);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<ArchivedTransactionDTO>(archivedTransaction);
        }
    }
}
