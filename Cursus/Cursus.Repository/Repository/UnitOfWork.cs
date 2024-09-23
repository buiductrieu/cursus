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
        public UnitOfWork(CursusDbContext db, ICategoryRepository categoryRepository)
        {
            _db = db;
            _categoryRepository = categoryRepository;
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
