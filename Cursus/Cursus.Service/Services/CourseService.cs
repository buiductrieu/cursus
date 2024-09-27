using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cursus.Service.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IUserService _userService;
        private readonly ICourseProgressService _progressService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository, ICourseProgressService progressService, IUserService userService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = courseRepository;
            _progressService = progressService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<PageListResponse<CourseResponseDTO>> GetCoursesAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page = 1, int pageSize = 20)
        {

            IEnumerable<Course> coursesRepo = await _repository.GetAllAsync(null, null);
            var courses = coursesRepo.ToList();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courses = courses.Where(p =>
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (p.Category != null && p.Category.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }


            if (sortOrder?.ToLower() == "desc")
            {
                courses = courses.OrderByDescending(GetSortProperty(sortColumn)).ToList();
            }
            else
            {
                courses = courses.OrderBy(GetSortProperty(sortColumn)).ToList();
            }


            var totalCount = courses.Count;


            var paginatedCourses = courses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return new PageListResponse<CourseResponseDTO>
            {
                Items = MapCoursesToDTOs(paginatedCourses),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasNextPage = (page * pageSize) < totalCount,
                HasPreviousPage = pageSize > 1
            };
        }
        private List<CourseResponseDTO> MapCoursesToDTOs(List<Course> courses)
        {
            List<CourseResponseDTO> courseDTOs = new List<CourseResponseDTO>();

            foreach (var course in courses)
            {
                var courseDTO = new CourseResponseDTO()
                {
                    Id = course.Id,
                    Name = course.Name,
                    Description = course.Description,
                    CategoryId = course.CategoryId,
                    DateCreated = course.DateCreated,
                    Status = course.Status,
                    Price = course.Price,
                    Discount = course.Discount,
                    StartedDate = course.StartedDate
                };

                courseDTOs.Add(courseDTO);
            }

            return courseDTOs;
        }




        private static Func<Course, object> GetSortProperty(string SortColumn)
        {
            return SortColumn?.ToLower() switch
            {
                "name" => course => course.Name,
                "description" => course => course.Description,
                "price" => course => course.Price,
                "categoryId" => course => course.CategoryId,
                "discount" => course => course.Discount,
                "dateCreated" => course => course.DateCreated,
                _ => course => course.Id

            };
        }

        public async Task<PageListResponse<CourseResponseDTO>> GetRegisteredCoursesByUserIdAsync(string userId, int page = 1, int pageSize = 20)
        {

            var userExists = await _userService.CheckUserExistsAsync(userId);
            if (!userExists)
            {
                return new PageListResponse<CourseResponseDTO>
                {
                    Items = new List<CourseResponseDTO>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0,
                    HasNextPage = false,
                    HasPreviousPage = false
                };
            }


            var courseIds = await _progressService.GetRegisteredCourseIdsAsync(userId);


            if (!courseIds.Any())
            {
                return new PageListResponse<CourseResponseDTO>
                {
                    Items = new List<CourseResponseDTO>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0,
                    HasNextPage = false,
                    HasPreviousPage = false
                };
            }


            var courseIdsSet = new HashSet<int>(courseIds);


            var courseList = await _repository.GetAllAsync();


            var filteredCourses = courseList.Where(c => courseIdsSet.Contains(c.Id));


            var totalCount = filteredCourses.Count();

            var paginatedCourses = filteredCourses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return new PageListResponse<CourseResponseDTO>
            {
                Items = MapCoursesToDTOs(paginatedCourses),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasNextPage = (page * pageSize) < totalCount,
                HasPreviousPage = page > 1
            };

        }

        public async Task<CourseDTO> CreateCourseWithSteps(CourseDTO courseDTO)
        {
            // Check unique name
            bool courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == courseDTO.Name);

            if (courseExists)
                throw new Exception("Course name must be unique.");

            if (courseDTO.Steps == null || !courseDTO.Steps.Any())
                throw new Exception("Steps cannot be empty.");

            var course = _mapper.Map<Course>(courseDTO);

            // Save course in db
            await _unitOfWork.CourseRepository.AddAsync(course);
            await _unitOfWork.SaveChanges();

            // Get back data of Course with steps in db
            var courseDB = await _unitOfWork.CourseRepository.GetAllIncludeStepsAsync(course.Id);

            // Map back to courseDTO
            var savedCourseDTO = _mapper.Map<CourseDTO>(courseDB);
            return savedCourseDTO;
        }

    }
}
