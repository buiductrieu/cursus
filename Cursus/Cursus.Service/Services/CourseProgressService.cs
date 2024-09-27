using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    internal class CourseProgressService : ICourseProgressService
    {
        private readonly IProgressRepository _progressRepository;

        public CourseProgressService(IProgressRepository progressRepository)
        {
            _progressRepository = progressRepository;
        }

      
        public async Task<IEnumerable<int>> GetRegisteredCourseIdsAsync(string userId)
        {
            var courseProgressList = (await _progressRepository.GetAllAsync()).AsQueryable();
            return courseProgressList
                .Where(p => p.UserId == userId)
                .Select(p => p.CourseId)
                .ToList();
        }
    }
    
}
