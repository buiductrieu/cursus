using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class InstructorCertificateRepository : Repository<InstructorCertificate>, IInstructorCertificateRepository

    {
        private readonly CursusDbContext _db;
        public InstructorCertificateRepository(CursusDbContext db) : base(db) => _db = db;

        public async Task<InstructorCertificate> FirstOrDefaultAsync(Expression<Func<InstructorCertificate, bool>> predicate)
        {
            return await _db.InstructorCertificates.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<InstructorCertificate>> GetInstructorCertificatesByInstructorIdAsync(int instructorId)
        {
            return await _db.InstructorCertificates
                .Where(c => c.InstructorId == instructorId)
                .ToListAsync();
        }

    }

}
