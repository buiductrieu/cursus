using Cursus.Data.DTO;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class InstructorDashboardService : IInstructorDashboardService
    {
        private readonly IInstructorDashboardRepository _instructorDashboardRepository;

        public InstructorDashboardService(IInstructorDashboardRepository instructorDashboardRepository)
        {
            _instructorDashboardRepository = instructorDashboardRepository;
        }

        public async Task<InstructorDashboardDTO> GetInstructorDashboardAsync(int instructorId)
        {
            var dashboardData = await _instructorDashboardRepository.GetInstructorDashboardAsync(instructorId);
            return dashboardData;
        }
    }

}
