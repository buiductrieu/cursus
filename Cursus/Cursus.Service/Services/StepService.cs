using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class StepService : IStepService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStepRepository _stepRepository;

        public StepService(IStepRepository stepRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _stepRepository = stepRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<StepDTO> CreateStep(CreateStepDTO dto)
        {
            //Validate....

            var stepEntity = _mapper.Map<Step>(dto); 
            if (stepEntity.DateCreated == DateTime.MinValue)
            {
                stepEntity.DateCreated = DateTime.UtcNow;
            }

            await _stepRepository.AddAsync(stepEntity);
            await _unitOfWork.SaveChanges();

            var createdStepDTO = _mapper.Map<StepDTO>(stepEntity);

            return createdStepDTO;
        }

        public async Task<StepDTO> GetStepByIdAsync(int id)
        {
            var step = await _unitOfWork.StepRepository.GetByIdAsync(id);

            if (step == null)
            {
                throw new KeyNotFoundException("Step not found.");
            }

            var stepDTO = _mapper.Map<StepDTO>(step);

            return stepDTO;
        }

        public async Task<bool> DeleteStep(int stepId)
        {
            var existingStep = await _unitOfWork.StepRepository.GetAsync(s => s.Id == stepId);
            if (existingStep == null)
                throw new KeyNotFoundException("Step not found.");

            await _unitOfWork.StepRepository.DeleteAsync(existingStep);
            await _unitOfWork.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<StepDTO>> GetStepsByCoursId(int courseId)
        {
            var steps = await _unitOfWork.StepRepository.GetStepsByCoursId(courseId);

            if (steps == null || !steps.Any())
            {
                throw new KeyNotFoundException("No steps found for the specified course.");
            }

            var stepsDTO = _mapper.Map<IEnumerable<StepDTO>>(steps);

            return stepsDTO;
        }

        public async Task<StepDTO> UpdateStep(StepUpdateDTO updateStepDTO)
        {
            var step = await _unitOfWork.StepRepository.GetByIdAsync(updateStepDTO.Id);

            if (step == null)
            {
                throw new KeyNotFoundException("Step not found.");
            }

            _mapper.Map(updateStepDTO, step);

            await _unitOfWork.StepRepository.UpdateAsync(step);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StepDTO>(step);
        }

    }
}
