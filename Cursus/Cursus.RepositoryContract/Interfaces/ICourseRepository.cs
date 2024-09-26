using Cursus.Data.Entities;
using System.Linq.Expressions;

namespace Cursus.RepositoryContract.Interfaces
{
	public interface ICourseRepository : IRepository<Course>
    {
		Task<bool> AnyAsync(Expression<Func<Course, bool>> predicate); 
		Task<Course> GetAllIncludeStepsAsync(int courseId);
	}
}
