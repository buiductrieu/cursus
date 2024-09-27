using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
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
        IUserRepository UserRepository { get; }
        IInstructorInfoRepository InstructorInfoRepository { get; }

        ICourseRepository CourseRepository { get; }

        IStepRepository StepRepository { get; }
        Task SaveChanges();
    }
}
