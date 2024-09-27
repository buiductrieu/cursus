using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Repository.Repository;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager))
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
             _userManager = userManager;
        }
        public async Task<UserProfileUpdateDTO> UpdateUserProfile(string id, UserProfileUpdateDTO usr)
        {
            // Retrieve the existing user profile
            var O_user = await _userRepository.ExiProfile(id);
            if (O_user == null)
            {
                throw new Exception("User not found");
            }

            // Update the properties of the existing user
            if (O_user.UserName != usr.UserName && await _userRepository.UsernameExistsAsync(usr.UserName))
            {
                throw new Exception("Username already exists");
            }
            O_user.Address = usr.Address;
            O_user.PhoneNumber = usr.PhoneNumber;
            if(O_user.EmailConfirmed)
        {
                O_user.Email = usr.Email;
        }
            // Update the user profile in the repository
            await _userRepository.UpdProfile(O_user);

            // Save changes to the database
            await _unitOfWork.SaveChanges();
      
            // Map the updated user entity back to the DTO
            var userDTO = _mapper.Map<UserProfileUpdateDTO>(O_user);
            return userDTO;
        }



        public async Task<bool> CheckUserExistsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }
    }
}
