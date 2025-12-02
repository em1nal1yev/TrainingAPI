using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Services
{
    public interface ITrainingService
    {
        Task<List<TrainingGetDto>> GetAllAsync();
        Task<TrainingGetDto?> GetByIdAsync(Guid id);
        Task<List<TrainingSessionGetDto>> GetSessionsByTrainingIdAsync(Guid trainingId);
        Task<TrainingSessionGetDto> CreateSessionAsync(TrainingSessionCreateDto sessionDto);
        Task CreateAsync(TrainingCreateDto dto);
        Task UpdateAsync(TrainingUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
