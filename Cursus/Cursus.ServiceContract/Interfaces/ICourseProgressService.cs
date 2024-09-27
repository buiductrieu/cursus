using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface ICourseProgressService 
    {
        Task<IEnumerable<int>> GetRegisteredCourseIdsAsync(string userId);
    }
}
