using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
	{
		private readonly CursusDbContext _db;
		public ICategoryRepository CategoryRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IStepRepository StepRepository { get; }
        public IUserRepository UserRepository { get; }


        public UnitOfWork(CursusDbContext db, ICategoryRepository categoryRepository, ICourseRepository courseRepository, IStepRepository stepRepository, IUserRepository userRepository)
		{
			_db = db;
            CategoryRepository = categoryRepository;
            CourseRepository = courseRepository;
            StepRepository = stepRepository;
			UserRepository = userRepository;
		}



        private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					_db.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public async Task SaveChanges()
		{
			await _db.SaveChangesAsync();
		}
	}
}
