using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string? Token, string? ErrorMessage)> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Username);
            if (user == null) return (false, null, "User not found");
            var result = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (!result) return (false, null, "Invalid Password");
            var token = await GenerateJwtToken(user);
            return (true, token, null);
        
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), //Thông tin chủ thể của object: tên đăng nhập của user
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),// unique identifier giúp phân biệt các token khác nhau, Sử dụng NewGuid() để tạo ra một giá trị đi nhất
                new Claim(ClaimTypes.NameIdentifier, user.Id) //Id để xác định người dùng 1 cách duy nhất 
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<ApplicationUser> RegisterAsync(UserRegisterDTO dto)
        {
            var user = _mapper.Map<ApplicationUser>(dto);   
            var result = await _userManager.CreateAsync(user, dto.Password);
            
            if (result.Succeeded == true)
            {
                await _unitOfWork.SaveChanges();
                var userForReturn = await _userManager.FindByEmailAsync(user.Email);
                return user;
            }
            else
            {
                throw new Exception("User not created");
            }
        }

        public async Task<bool> ConfirmEmail(string username, string token)
        {
            var user = await _userManager.FindByEmailAsync(username) ?? throw (new Exception("User not found"));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
    }
}

