using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.Training;

namespace TelimAPI.Application.Services
{
    public interface ITrainingService
    {
        Task<List<TrainingGetDto>> GetAllAsync();
        Task<TrainingGetDto?> GetByIdAsync(Guid id);
        Task CreateAsync(TrainingCreateDto dto);
        Task UpdateAsync(TrainingUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
