using Cursus.Data.DTO;
using Cursus.Data.Entities;
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
        Task<bool> ApproveInstructorAsync(string instructorId);
        Task<bool> RejectInstructorAsync(string instructorId);
        Task<IEnumerable<InstructorInfo>> GetAllInstructors();

    }
}
