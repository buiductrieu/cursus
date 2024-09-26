using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;

namespace Cursus.Service.Services
{
	public class CourseService : ICourseService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
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
