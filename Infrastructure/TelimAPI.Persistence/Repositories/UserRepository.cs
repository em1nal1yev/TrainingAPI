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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Court)
                .Include(u => u.TrainingParticipants)
                    .ThenInclude(u => u.Training)
                        .ThenInclude(u => u.TrainingCourts)
                            .ThenInclude(u => u.Court)
                .Include(u => u.TrainingParticipants)
                    .ThenInclude(u => u.Training)
                        .ThenInclude(u => u.TrainingDepartments)
                            .ThenInclude(u => u.Department)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Court)
                .Include(u => u.TrainingParticipants)
                    .ThenInclude(u => u.Training)
                        .ThenInclude(u => u.TrainingCourts)
                            .ThenInclude(u => u.Court)
                .Include(u => u.TrainingParticipants)
                    .ThenInclude(u => u.Training)
                        .ThenInclude(u => u.TrainingDepartments)
                            .ThenInclude(u => u.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user != null)
            {
                _context.Remove(user);
                await _context.SaveChangesAsync();
            }
        }


    }
}
