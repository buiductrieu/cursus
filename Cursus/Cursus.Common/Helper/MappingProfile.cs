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

			CreateMap<UserProfileUpdateDTO,ApplicationUser>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
				.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

				
			CreateMap<Course, CourseDTO>();
			CreateMap<Step, StepDTO>();
			CreateMap<ApplicationUser, UserProfileUpdateDTO>();
		}
	}
}
