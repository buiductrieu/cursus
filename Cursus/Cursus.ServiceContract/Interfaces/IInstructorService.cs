﻿using Cursus.Data.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IInstructorService
    {
        Task<IdentityResult> InstructorAsync(RegisterInstructorDTO registerInstructorDTO);
    }
}