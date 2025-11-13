using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Repositories;
using TelimAPI.Domain.Entities;
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
               .FirstOrDefaultAsync(t => t.Id == id);

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
    }
}
