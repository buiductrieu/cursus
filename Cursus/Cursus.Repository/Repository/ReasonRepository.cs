using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class ReasonRepository : Repository<Reason>, IReasonRepository
    {
        private readonly CursusDbContext _db
            ;
        public ReasonRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
