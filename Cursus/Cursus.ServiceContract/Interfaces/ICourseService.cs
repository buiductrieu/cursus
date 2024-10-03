using Cursus.Data.DTO;

namespace Cursus.ServiceContract.Interfaces
{
	public interface ICourseService
	{
        Task<PageListResponse<CourseDTO>> GetCoursesAsync(string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 20);
        

        Task<PageListResponse<CourseDTO>> GetRegisteredCoursesByUserIdAsync(string userId, int page = 1, int pageSize = 20);
		Task<CourseDTO> CreateCourseWithSteps(CourseDTO courseDTO);

        Task<CourseDTO> UpdateCourseWithSteps(CourseDTO courseDTO);

        Task<bool> DeleteCourse(int courseId);
    }
}
