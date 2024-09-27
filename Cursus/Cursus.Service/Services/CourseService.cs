using Azure.Core;
using Cursus.Data.DTO.CourseDTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IUserService _userService;
        private readonly ICourseProgressService _progressService;

        public CourseService(ICourseRepository courseRepository, ICourseProgressService progressService, IUserService userService)
        {
            _repository = courseRepository;
            _progressService = progressService;
             _userService = userService;

        }


        
            public async Task<PageListResponse<CourseResponseDTO>> GetCoursesAsync(string? searchTerm,string? sortColumn,string?sortOrder,int page = 1,int pageSize = 20)
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

       
        
    }
}
