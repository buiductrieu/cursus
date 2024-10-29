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
            //User mapper
            CreateMap<ApplicationUser, UserProfileUpdateDTO>();

            CreateMap<UserRegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()).ReverseMap();

            CreateMap<UserProfileUpdateDTO, ApplicationUser>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
              .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

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


			//Course mapper
			CreateMap<CourseCreateDTO, Course>()
			   .ForMember(dest => dest.InstructorInfoId, opt => opt.MapFrom(src => src.InstructorInfoId))
			   .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date))
			   .ForMember(dest => dest.StartedDate, opt => opt.MapFrom(src => DateTime.UtcNow.Date))
			   .ForMember(dest => dest.DateModified, opt => opt.MapFrom(src => DateTime.UtcNow.Date))
			   .ForMember(dest => dest.InstructorInfoId, opt => opt.MapFrom(src => src.InstructorInfoId))
			   ;


            CreateMap<Course, CourseDTO>()
               .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.InstructorInfoId))
               .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps));

            CreateMap<CourseDTO, CourseUpdateDTO>()
               .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.StartedDate, opt => opt.MapFrom(src => src.StartedDate))
               .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps))
               .ForMember(dest => dest.DateModified, opt => opt.Ignore()); // Nếu DateModified không cần từ CourseDTO

            CreateMap<CourseUpdateDTO, CourseDTO>()
              .ForMember(dest => dest.Rating, opt => opt.Ignore()); // Nếu không cần Rating trong CourseUpdateDTO

            // Step Mapping
            CreateMap<StepCreateDTO, Step>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow.Date));

            CreateMap<StepUpdateDTO, Step>()
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)); // Chỉ định ID của Step để cập nhật

            CreateMap<Step, StepUpdateDTO>().ReverseMap();  // Để có thể lấy lại thông tin nếu cần
            CreateMap<Step, StepDTO>().ReverseMap();

            // CreateReasonMapper
            CreateMap<CreateReasonDTO, Reason>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCancel, opt => opt.MapFrom(src => DateTime.UtcNow));
            // Reason Mapping
            CreateMap<ReasonDTO, Reason>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.DateCancel, opt => opt.MapFrom(src => DateTime.UtcNow.Date));

            /*CreateMap<StepUpdateDTO, Step>()
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)); // Chỉ định ID của Step để cập nhật

            CreateMap<Step, StepUpdateDTO>().ReverseMap();  // Để có thể lấy lại thông tin nếu cần*/
            CreateMap<Reason, ReasonDTO>().ReverseMap();

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

            CreateMap<CourseCommentDTO, CourseComment>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            // Map from StepCommentCreateDTO to StepComment (for creating new comments)
            CreateMap<StepCommentCreateDTO, StepComment>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.StepId, opt => opt.MapFrom(src => src.StepId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore()); // DateCreated should be set manually

            // Map from StepComment to StepCommentDTO (for displaying comment details)
            CreateMap<StepComment, StepCommentDTO>()
                .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.StepName, opt => opt.MapFrom(src => src.Step.Name))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated));

            CreateMap<Course, CourseDetailDTO>()
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps));

            //Cart Mapping
            CreateMap<CartDTO, Cart>().ReverseMap();

            //CartItems Mapping
            CreateMap<CartItems, CartItemsDTO>()
                .ForPath(dest => dest.Name, opt => opt.MapFrom(src => src.Course.Name)).ReverseMap();

            CreateMap<StepCommentCreateDTO, StepComment>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.StepId, opt => opt.MapFrom(src => src.StepId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow));

			// Mapping từ Transaction sang TransactionDTO
			CreateMap<Transaction, TransactionDTO>()
				.ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
				.ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
			//Oder Mapping
			CreateMap<Order, OrderDTO>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ReverseMap();

            CreateMap<InstructorInfo, InstuctorTotalEarnCourseDTO>()
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Earnings, opt => opt.Ignore())
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count));

            //Bookmark Mapping
            CreateMap<BookmarkCreateDTO, Bookmark>();

            CreateMap<Bookmark, BookmarkDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Course.Price))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Course.Rating));


            //Wallet Mapping
            CreateMap<Wallet, WalletDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated));

            //RegisterInsstructor Mapping
            CreateMap<RegisterInstructorDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName)) 
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) 
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore()) 
                .ReverseMap();


            CreateMap<PayoutRequest, PayoutRequestDisplayDTO>()
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.User.UserName))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Transaction.Amount))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PayoutRequestStatus));
        }
    }
}
