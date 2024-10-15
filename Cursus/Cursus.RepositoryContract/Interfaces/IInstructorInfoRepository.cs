using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Data.Entities;
using System.Linq.Expressions;
using Cursus.Data.DTO;
using Cursus.RepositoryContract.Interfaces;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IInstructorInfoRepository : IRepository<InstructorInfo>
    {
        Task<IEnumerable<InstructorInfo>> GettAllAsync();

        Task DeleteAsync(int id);
        Task<IEnumerable<InstructorInfo>> GetAllInstructorsAsync();


    }
}