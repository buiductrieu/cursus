using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;

namespace Cursus.Repository.Repository
{
	public class StepRepository : Repository<Step>, IStepRepository
	{
		private readonly CursusDbContext _db;
		public StepRepository(CursusDbContext db) : base(db) => _db = db;
	}
}
