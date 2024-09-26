using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;

namespace Cursus.Common.Helper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<CourseDTO, Course>()
			   .ForMember(dest => dest.Id, opt => opt.Ignore())
			   .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date));

			CreateMap<StepDTO, Step>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CourseId, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date));
				
			CreateMap<Course, CourseDTO>();
			CreateMap<Step, StepDTO>();
		}
	}
}
