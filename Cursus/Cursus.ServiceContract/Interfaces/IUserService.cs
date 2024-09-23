using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IUserService
    {
        public Task<ApplicationUser> UpdateUserProfile(string id, ApplicationUser usr);
    }
}
