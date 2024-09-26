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
		private ICategoryRepository _categoryRepository;
		private ICourseRepository _courseRepository;
		private IStepRepository _stepRepository;
		private IUserRepository _userRepository;

		public UnitOfWork(CursusDbContext db, ICategoryRepository categoryRepository, ICourseRepository courseRepository, IStepRepository stepRepository, IUserRepository userRepository)
		{
			_db = db;
			_categoryRepository = categoryRepository;
			_courseRepository = courseRepository;
			_stepRepository = stepRepository;
			_userRepository = userRepository;
		}


		public ICategoryRepository CategoryRepository
		{
			get
			{
				if (_categoryRepository == null)
				{
					_categoryRepository = new CategoryRepository(_db);
				}
				return _categoryRepository;
			}
		}

		public ICourseRepository CourseRepository
		{
			get
			{
				if (_courseRepository == null)
				{
					_courseRepository = new CourseRepository(_db);
				}
				return _courseRepository;
			}
		}

		public IStepRepository StepRepository
		{
			get
			{
				if (_stepRepository == null)
				{
					_stepRepository = new StepRepository(_db);
				}
				return _stepRepository;
			}
		}
        public IUserRepository userRepositiory
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_db);
                }
                return _userRepository;
            }
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
