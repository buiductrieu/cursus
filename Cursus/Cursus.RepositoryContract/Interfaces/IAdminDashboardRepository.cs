using Cursus.Data.DTO;
using Cursus.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<List<PurchaseCourseOverviewDTO>> GetTopPurchasedCourses(int year, string period);

    }
}
