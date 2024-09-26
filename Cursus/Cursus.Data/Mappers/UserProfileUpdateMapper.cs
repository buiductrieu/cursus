using Cursus.Data.DTO;
using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Mapper
{
    public static class UserProfileUpdateMapper
    {
        public static ApplicationUser MapToEntity(this UserProfileUpdateDTO dto)
        {
            return new ApplicationUser
            {
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                UserName = dto.UserName
            };
        }
    }
}
