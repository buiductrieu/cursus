using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IUserRepository userRepositiory { get; }

        ICourseRepository CourseRepository { get; }

        IStepRepository StepRepository { get; }
        Task SaveChanges();
    }
}
