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
        Task<List<TrainingGetDto>> GetExpiredAsync();
        Task<List<TrainingGetDto>> GetDraftsAsync();
        Task<List<TrainingOngoingWithUsersDto>> GetOngoingAsync();
        Task<bool> ApproveAsync(Guid id);
        Task<List<TrainingSessionGetDto>> GetSessionsByTrainingIdAsync(Guid trainingId);
        Task<TrainingSessionGetDto> CreateSessionAsync(TrainingSessionCreateDto sessionDto);
        Task CreateAsync(TrainingCreateDto dto);
        Task UpdateAsync(TrainingUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<List<SessionParticipantDto>> GetSessionAttendanceListAsync(Guid sessionId);
        Task AddSessionAttendanceAsync(Guid sessionId, List<SessionAttendanceDto> attendanceDtos);
        Task<List<Guid>> GetJoinedParticipantIdsAsync(Guid trainingId);
        Task<SessionDetailsDto> GetSessionDetailsWithParticipantsAsync(Guid sessionId);

    }
}
