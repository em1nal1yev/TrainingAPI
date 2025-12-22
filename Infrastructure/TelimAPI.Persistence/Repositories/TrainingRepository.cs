using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Repositories;
using TelimAPI.Domain.Entities;
using TelimAPI.Domain.Enums;
using TelimAPI.Persistence.Contexts;

namespace TelimAPI.Persistence.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly AppDbContext _context;
        public TrainingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Training>> GetAllAsync()
        {
            return await _context.Trainings
                .Include(t => t.Participants)
                .Include(t => t.TrainingCourts)
                    .ThenInclude(tc => tc.Court)
                .Include(t => t.TrainingDepartments)
                    .ThenInclude(td => td.Department)
                .ToListAsync();
        }

        public async Task<Training?> GetByIdAsync(Guid id)
        {
            return await _context.Trainings
               .Include(t => t.TrainingCourts)
                   .ThenInclude(tc => tc.Court)
               .Include(t => t.TrainingDepartments)
                   .ThenInclude(td => td.Department)
               .Include(t => t.Participants)
               .FirstOrDefaultAsync(t => t.Id == id);

        }

        public async Task<List<Training>> GetDraftsAsync()
        {
            return await _context.Trainings
                .Where(x => x.Status == TrainingStatus.Draft)
                .ToListAsync();
        }

        public async Task<List<Training>> GetOngoingAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Trainings
                .Include(x => x.Participants)
                    .ThenInclude(p => p.User)
                .Where(t => t.Status == TrainingStatus.OnGoing)
                .ToListAsync();
        }   

        public async Task AddAsync(Training training)
        {
            await _context.Trainings.AddAsync(training);
            await _context.SaveChangesAsync();
        }
        public void Update(Training training)
        {
            _context.Trainings.Update(training);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(Guid id)
        {
            var training = await _context.Trainings.FirstOrDefaultAsync(x => x.Id == id);
            if (training != null)
            {
                _context.Trainings.Remove(training);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TrainingParticipant?> GetParticipantByTrainingAndUserAsync(Guid trainingId, Guid userId)
        {
            return await _context.TrainingParticipants

        .FirstOrDefaultAsync(p => p.TrainingId == trainingId && p.UserId == userId);
        }

        public async Task AddParticipantAsync(TrainingParticipant participant)
        {
            await _context.TrainingParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateParticipantAsync(TrainingParticipant participant)
        {
            _context.TrainingParticipants.Update(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<TrainingFeedback?> GetFeedbackByParticipantIdAsync(Guid participantId)
        {
            return await _context.TrainingFeedback
                .FirstOrDefaultAsync(f => f.TrainingParticipantId == participantId);
        }

        public async Task AddFeedbackAsync(TrainingFeedback feedback)
        {
            await _context.TrainingFeedback.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TrainingSession>> GetSessionsByTrainingIdAsync(Guid trainingId)
        {
                return await _context.TrainingSessions
                    .Where(s => s.TrainingId == trainingId)
                    .Include(s => s.Attendances) 
                    .ToListAsync();
        }

        public async Task AddTrainingSessionAsync(TrainingSession session)
        {
            await _context.TrainingSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TrainingParticipant>> GetJoinedParticipantsByTrainingIdAsync(Guid trainingId)
        {
            return await _context.Set<TrainingParticipant>()
                             .Include(p => p.User)
                             .Where(p => p.TrainingId == trainingId && p.IsJoined == true)
                             .ToListAsync();
        }

        public async Task AddRangeSessionAttendanceAsync(IEnumerable<SessionAttendance> attendances)
        {
            await _context.Set<SessionAttendance>().AddRangeAsync(attendances);
            await _context.SaveChangesAsync();
        }

        public async Task<SessionAttendance?> GetAttendanceBySessionAndUserAsync(Guid sessionId, Guid userId)
        {
            return await _context.Set<SessionAttendance>()
                                 .FirstOrDefaultAsync(a => a.TrainingSessionId == sessionId && a.UserId == userId);
        }

        public async Task UpdateSessionAttendance(SessionAttendance attendance)
        {
            _context.Set<SessionAttendance>().Update(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task<TrainingSession?> GetSessionByIdAsync(Guid sessionId)
        {
            return await _context.Set<TrainingSession>()
                             .Include(s => s.Training)
                                .ThenInclude(t => t.Participants.Where(p => p.IsJoined == true))
                                    .ThenInclude(p => p.User)
                             .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<List<SessionAttendance>> GetAllAttendancesBySessionIdAsync(Guid sessionId)
        {
            return await _context.Set<SessionAttendance>()
                             .Where(a => a.TrainingSessionId == sessionId)
                             .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Training>> GetExpiredAsync()
        {
            return await _context.Trainings
                .Include(x => x.TrainingCourts)
                    .ThenInclude(x => x.Court)
                .Include(x => x.TrainingDepartments)
                    .ThenInclude(x => x.Department)
                .Where(t => t.EndDate < DateTime.UtcNow)
                .ToListAsync();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
