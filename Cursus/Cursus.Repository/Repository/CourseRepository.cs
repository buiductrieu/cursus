using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
	public class CourseRepository : Repository<Course>, ICourseRepository
	{
		private readonly CursusDbContext _db;
		public CourseRepository(CursusDbContext db) : base(db) => _db = db;
		public async Task<bool> AnyAsync(Expression<Func<Course, bool>> predicate)
		{
			return await _db.Set<Course>().AnyAsync(predicate);
		}
		public async Task<Course> GetAllIncludeStepsAsync(int courseId)
		{
			var course = await _db.Set<Course>().FirstOrDefaultAsync(c => c.Id == courseId);
			return course!;
		}
	}
}
