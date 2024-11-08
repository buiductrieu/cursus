using AutoMapper;
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
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminDashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PurchaseCourseOverviewDTO>> GetTopPurchasedCourses(int year, string period)
        {
            var topPurchasedCourses = await _unitOfWork.AdminDashboardRepository.GetTopPurchasedCourses(year, period);

            var topPurchasedCoursesDto = _mapper.Map<List<PurchaseCourseOverviewDTO>>(topPurchasedCourses);

            return topPurchasedCoursesDto;
        }
    }
}
