using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.ServiceContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class InstructorRepository : Repository<InstructorInfo>, IInstructorInfoRepository
    { 
        private readonly CursusDbContext _dbContext;

        public InstructorRepository(CursusDbContext dbContext ) :base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(InstructorInfo instructorInfo)
        {
            await _dbContext.InstructorInfos.AddAsync(instructorInfo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var instructor = await _dbContext.InstructorInfos.FindAsync(id);
            if (instructor != null)
            {
                _dbContext.InstructorInfos.Remove(instructor);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<InstructorInfo>> GetAllInstructorsAsync()
        {
            return await _dbContext.InstructorInfos.Include(i => i.User).ToListAsync();
        }

        public async Task<InstructorInfo> GetByIDAsync(int id)
        {
           return await _dbContext.InstructorInfos.Include(i => i.User).FirstOrDefaultAsync(i => i.Id == id) ?? throw new KeyNotFoundException("Instructor not found");
        }

        public async Task<IEnumerable<InstructorInfo>> GettAllAsync()
        {
           return await _dbContext.InstructorInfos.Include(i => i.User).ToListAsync();
        }

        public async Task UpdateAsync(InstructorInfo instructorInfo)
        {
            _dbContext.InstructorInfos.Update(instructorInfo);
            await _dbContext.SaveChangesAsync();
        }
    }
}
