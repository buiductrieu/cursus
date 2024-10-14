using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;

namespace Cursus.Repository.Repository
{
	public class CourseProgressRepository : Repository<CourseProgress>, ICourseProgressRepository
	{
		private readonly CursusDbContext _db;
		public CourseProgressRepository(CursusDbContext db) : base(db) => _db = db;
	}
}
