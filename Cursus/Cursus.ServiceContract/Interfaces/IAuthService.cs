﻿using Cursus.Data.DTO;
using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string? Token, string? ErrorMessage)> LoginAsync(LoginRequestDTO loginRequestDTO);
        public Task<ApplicationUser> RegisterAsync(UserRegisterDTO dto);
        Task<bool> ConfirmEmail(string username, string token);
    }
}