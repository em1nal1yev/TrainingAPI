using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Services
{
    public interface ITrainingService
    {
        Task<List<TrainingGetDto>> GetAllAsync();
        Task<Result<TrainingGetDto>?> GetByIdAsync(Guid id);
        Task<List<TrainingGetDto>> GetExpiredAsync();
        Task<List<TrainingGetDto>> GetDraftsAsync();
        Task<List<TrainingOngoingWithUsersDto>> GetOngoingAsync();
        Task<Result> ApproveAsync(Guid id);
        Task<Result<List<TrainingSessionGetDto>>> GetSessionsByTrainingIdAsync(Guid trainingId);
        Task<Result<TrainingSessionGetDto>> CreateSessionAsync(TrainingSessionCreateDto sessionDto);
        Task<Result> CreateAsync(TrainingCreateDto dto);
        Task<Result> UpdateAsync(TrainingUpdateDto dto);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> AddSessionAttendanceAsync(Guid sessionId, List<SessionAttendanceDto> attendanceDtos);
        Task<Result<SessionDetailsDto>> GetSessionDetailsWithParticipantsAsync(Guid sessionId);
        Task<Result<List<HighAttendanceDto>>> GetHighAttendanceAsync(Guid trainingId);
        Task<Result<List<HighAttendanceDto>>> GetLowAttendanceAsync(Guid trainingId);
        Task<Result<TrainingAttendanceSummaryDto>> GetTrainingAttendancesAsync(Guid trainingId);

    }
}
