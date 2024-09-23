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
        public UnitOfWork(CursusDbContext db)
        {
            _db = db;
        }

        private ICategoryRepository _categoryRepository;
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

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }
    }
}
