using Cursus.Data.DTO;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IArchivedTransactionService
    {
        public Task<ArchivedTransactionDTO> ArchiveTransaction(int transactionId);
    }
}
