using Cursus.Data.DTO;
using Cursus.Data.Entities;
using System.Linq.Expressions;

namespace Cursus.ServiceContract.Interfaces
{
	public interface ICourseService
	{
		Task<CourseDTO> CreateCourseWithSteps(CourseDTO courseDTO);
	}
}
