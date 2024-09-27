using Cursus.Data.DTO.CourseDTO;
using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface ICourseService
    {
        Task<PageListResponse<CourseResponseDTO>> GetCoursesAsync(string? searchTerm,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 20);
       
        Task<PageListResponse<CourseResponseDTO>> GetRegisteredCoursesByUserIdAsync(string userId, int page = 1, int pageSize = 20);
    }
}
