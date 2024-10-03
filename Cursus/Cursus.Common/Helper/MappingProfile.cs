using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.DTO.Category;
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

			//Course mapper
			CreateMap<CourseDTO, Course>()
			   .ForMember(dest => dest.Id, opt => opt.Ignore())
			   .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date));

            CreateMap<StepDTO, Step>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date));

            CreateMap<UserProfileUpdateDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

			CreateMap<Course, CourseDTO>();
			CreateMap<Step, StepDTO>();

			//User mapper
			CreateMap<ApplicationUser, UserProfileUpdateDTO>();
		
			//Category mapper
            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for new records
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date)); 

            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore()); 

            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses)); 

            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.Courses, opt => opt.Ignore());
            //

            // StepContent Mapping
            CreateMap<StepContentDTO, StepContent>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore the Id field for creation
                .ForMember(dest => dest.StepId, opt => opt.MapFrom(src => src.StepId)) // StepId mapping
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.ContentURL, opt => opt.MapFrom(src => src.ContentURL))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated != default(DateTime) ? src.DateCreated : DateTime.UtcNow)) // Set DateCreated or default to UTC now
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<StepContent, StepContentDTO>()
                .ForMember(dest => dest.StepId, opt => opt.MapFrom(src => src.StepId)) // Mapping StepId back
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.ContentURL, opt => opt.MapFrom(src => src.ContentURL))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated)); // Map DateCreated back to DTO


            CreateMap<Course, CourseDTO>();
            
            CreateMap<Step, StepDTO>();
            
            CreateMap<ApplicationUser, UserProfileUpdateDTO>();
            
            CreateMap<UserRegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()).ReverseMap();
            
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            //CourseComment Mapping
            CreateMap<CourseComment, CourseCommentCreateDTO>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ReverseMap();

            CreateMap<CourseComment, CourseCommentDTO>()
               .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
               .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateCreated))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
	}
}
