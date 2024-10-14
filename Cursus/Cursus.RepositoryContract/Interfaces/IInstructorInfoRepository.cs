using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Data.Entities;
using System.Linq.Expressions;
using Cursus.Data.DTO;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IInstructorInfoRepository
    {
        Task<IEnumerable<InstructorInfo>> GettAllAsync();
        Task<InstructorInfo> GetByIDAsync(int id);

        Task AddAsync(InstructorInfo instructorInfo);

        Task UpdateAsync(InstructorInfo instructorInfo);

        Task DeleteAsync(int id);
        Task<IEnumerable<InstructorInfo>> GetAllInstructorsAsync();


    }
}