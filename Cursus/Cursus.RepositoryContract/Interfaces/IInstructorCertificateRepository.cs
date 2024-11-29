using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IInstructorCertificateRepository : IRepository<InstructorCertificate>
    {
        Task<InstructorCertificate> FirstOrDefaultAsync(Expression<Func<InstructorCertificate, bool>> predicate);
        Task<IEnumerable<InstructorCertificate>> GetInstructorCertificatesByInstructorIdAsync(int instructorId);

    }
}
