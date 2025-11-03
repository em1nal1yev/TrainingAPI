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
    public class CourtRepository : ICourtRepository
    {
        private readonly AppDbContext _context;

        public CourtRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Court>> GetAllAsync()
        {
            return await _context.Courts
                .Include(c => c.Departments)
                .Include(c => c.Users)
                .Include(c => c.TrainingCourts)
                    .ThenInclude(c => c.Training)
                .ToListAsync();
        }

        public async Task<Court?> GetByIdAsync(Guid id)
        {
           return await _context.Courts
                .Include (c => c.Departments)
                .Include(c => c.Users)
                .Include(c => c.TrainingCourts)
                    .ThenInclude (c => c.Training)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
