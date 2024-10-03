using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class CourseCommentService : ICourseCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public CourseCommentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public Task<CourseCommentDTO> DeleteComment(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseCommentDTO>> GetCourseCommentsAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public async Task<CourseCommentDTO> PostComment(CourseCommentCreateDTO courseComment)
        {
            var user = await _userManager.FindByIdAsync(courseComment.UserId);

            if (_userManager.IsEmailConfirmedAsync(user).Result == false)
            {
                throw new UnauthorizedAccessException("Your email is not confirmed");
            }

            var comment = _mapper.Map<CourseComment>(courseComment);

            comment.Course = await _unitOfWork.CourseRepository.GetAsync(u => u.CategoryId == courseComment.CourseId);

            comment.User = user;

            var commentForReturn = _mapper.Map<CourseCommentDTO>(comment);

            await _unitOfWork.SaveChanges();

            return commentForReturn;
        }

    }
}
