using Cursus.Data.DTO;
using Cursus.Data.Entities;
using System.Linq.Expressions;

namespace Cursus.ServiceContract.Interfaces
{
	public interface ICourseService
	{
		Task<CourseDTO> CreateCourseWithSteps(CourseDTO courseDTO);

        Task<CourseDTO> UpdateCourseWithSteps(CourseDTO courseDTO);

        Task<bool> DeleteCourse(int courseId);
    }
}
