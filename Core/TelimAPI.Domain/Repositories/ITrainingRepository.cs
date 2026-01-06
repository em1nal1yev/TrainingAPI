using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Repositories
{
    public interface ITrainingRepository
    {
        Task<List<Training>> GetAllAsync();

        Task<Training?> GetByIdAsync(Guid id);
        Task<List<Training>> GetExpiredAsync();
        Task<List<Training>> GetDraftsAsync();
        Task<List<Training>> GetOngoingAsync();

        Task AddAsync(Training training);

        void Update(Training training);

        Task DeleteAsync(Guid id);

        Task<TrainingParticipant?> GetParticipantByTrainingAndUserAsync(Guid trainingId, Guid userId);
        Task AddParticipantAsync(TrainingParticipant participant);
        Task UpdateParticipantAsync(TrainingParticipant participant);

        Task<TrainingFeedback?> GetFeedbackByParticipantIdAsync(Guid participantId);
        Task AddFeedbackAsync(TrainingFeedback feedback);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<TrainingSession>> GetSessionsByTrainingIdAsync(Guid trainingId);
        Task AddTrainingSessionAsync(TrainingSession session);
        Task<List<TrainingParticipant>> GetJoinedParticipantsByTrainingIdAsync(Guid trainingId);
        Task AddRangeSessionAttendanceAsync(IEnumerable<SessionAttendance> attendances);
        Task<SessionAttendance?> GetAttendanceBySessionAndUserAsync(Guid sessionId, Guid userId);
        Task UpdateSessionAttendance(SessionAttendance attendance);
        Task<TrainingSession?> GetSessionByIdAsync(Guid sessionId);
        Task<List<SessionAttendance>> GetAllAttendancesBySessionIdAsync(Guid sessionId);
        Task SaveAsync();
    }
}
