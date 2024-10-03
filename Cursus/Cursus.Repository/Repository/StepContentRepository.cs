using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cursus.Repository.Repository
{

    public class StepContentRepository : Repository<StepContent>, IStepContentRepository
    {
        private readonly CursusDbContext _db;
        private DbSet<StepContent> _dbSet;

        public StepContentRepository(CursusDbContext db) : base(db)
        {
            _db = db;
            _dbSet = _db.Set<StepContent>();
        }

        public async Task<StepContent> GetByIdAsync(int id)
        {
            // Sử dụng phương thức FindAsync để tìm StepContent theo Id
            return await _db.StepContents
                                   .FirstOrDefaultAsync(sc => sc.Id == id) ?? throw new KeyNotFoundException("Step content is not found");
        }
    }
}

