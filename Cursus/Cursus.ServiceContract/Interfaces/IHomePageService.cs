using Cursus.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IHomePageService
    {
        Task<HomePageDTO> GetHomePageAsync();
        Task<HomePageDTO> UpdateHomePageAsync(int id, HomePageDTO homePageDto);
    }
}
